using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Winecrash.Engine
{
    public sealed class Shader : BaseObject
    {
        internal struct ShaderUniformData
        {
            public string Name { get; }
            public int Location { get; }
            public int Size { get; }
            public ActiveUniformType Type { get; }

            public ShaderUniformData(string name, int location, int size, ActiveUniformType type)
            {
                this.Name = name;
                this.Location = location;
                this.Size = size;
                this.Type = type;
            }
        }

        internal class ShaderAttributeData
        {
            public ShaderAttributeData(string name, int location, int size, int length, ActiveAttribType type)
            {
                this.Name = name;
                this.Location = location;
                this.Size = size;
                this.Length = length;
                this.Type = type;
            }

            public string Name { get; }
            public int Location { get; }
            public int Size { get; }
            public int Length { get; }
            public ActiveAttribType Type { get; }
        }

        internal class ShaderResourceData
        {

        }

        public int Handle { get; private set; } = -1;

        internal ShaderUniformData[] Uniforms { get; private set; }
        internal ShaderAttributeData[] Attributes { get; private set; }


        internal static List<Shader> Cache = new List<Shader>();

        public static Shader ErrorShader { get; private set; }

        public static Shader Find(string name)
        {
            return Cache.Find(s => s.Name == name);
        }

        internal static Shader CreateError()
        {
            const string frag =
            @"#version 330
            out vec4 outputColor;
            void main()
            {
                outputColor = vec4(1.0, 0.0, 1.0, 1.0);
            }";

            const string vert =
            @"#version 330 core
            layout(location = 0) in vec3 position;
            uniform mat4 transform;
            void main()
            {
                gl_Position = vec4(position, 1.0) * transform;
            }";

            return ErrorShader = new Shader("Error", vert, frag);
        }
        private Shader(string name, string vert, string frag) : base(name)
        {
            // define vertex shader
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vert);
            CompileShader(vertexShader);

            // define fragment shader
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, frag);
            CompileShader(fragmentShader);

            // create program
            this.Handle = GL.CreateProgram();

            // link shaders together and compile the program
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            LinkProgram(Handle);

            // cleaning
            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            GL.GetProgram(Handle, GetProgramParameterName.ActiveAttributes, out int nAttribs);
            Attributes = new ShaderAttributeData[nAttribs];

            for (int i = 0; i < nAttribs; i++)
            {
                GL.GetActiveAttrib(Handle, i, 16, out int ALength, out int ASize, out ActiveAttribType AType, out string AName);


                
                this.Attributes[i] = new ShaderAttributeData(AName, i, ASize, ALength, AType);
            }


            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int nUniforms);
            Uniforms = new ShaderUniformData[nUniforms];

            for (int i = 0; i < nUniforms; i++)
            {
                string UName = GL.GetActiveUniform(Handle, i, out int USize, out ActiveUniformType UType);
                Uniforms[i] = new ShaderUniformData(UName, GL.GetUniformLocation(Handle, UName), USize, UType);
            }


            Cache.Add(this);
        }

        public Shader(string vertexPath, string fragmentPath) : base()
        {
            string[] reps = vertexPath.Split('/', '\\');
            this.Name = reps[reps.Length - 1].Split('.')[0];

            string shaderSource = LoadSource(vertexPath);
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, shaderSource);
            CompileShader(vertexShader);

            shaderSource = LoadSource(fragmentPath);
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            CompileShader(fragmentShader);

            this.Handle = GL.CreateProgram();

            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);

            LinkProgram(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);


            GL.GetProgram(Handle, GetProgramParameterName.ActiveAttributes, out int nAttribs);
            Attributes = new ShaderAttributeData[nAttribs];
            
            for (int i = 0; i < nAttribs; i++)
            {
                GL.GetActiveAttrib(Handle, i, 16, out int ALength, out int ASize, out ActiveAttribType AType, out string AName);

                this.Attributes[i] = new ShaderAttributeData(AName, GL.GetAttribLocation(Handle, AName), ASize, ALength, AType);
            }
            

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int nUniforms);
            Uniforms = new ShaderUniformData[nUniforms];

            for (int i = 0; i < nUniforms; i++)
            {
                string UName = GL.GetActiveUniform(Handle, i, out int USize, out ActiveUniformType UType);
                Uniforms[i] = new ShaderUniformData(UName, GL.GetUniformLocation(Handle, UName), USize, UType);
            }

            SetAttribute("position", AttributeTypes.Vertice);
            SetAttribute("uv", AttributeTypes.UV);
            SetAttribute("normal", AttributeTypes.Normal);

            Cache.Add(this);
        }

        internal bool SetAttribute(string name, AttributeTypes type)
        {
            ShaderAttributeData data = null;

            for (int i = 0; i < Attributes.Length; i++)
            {
                if (Attributes[i].Name == name)
                {
                    data = Attributes[i];
                }

            }

            if (data == null) return false;

            GL.EnableVertexAttribArray(data.Location);

            int size;
            VertexAttribPointerType ptrType;
            bool normalized;
            int stride;
            int offset;

            const int VerticeSize = 3;
            const int UVSize = 2;
            const int NormalSize = 3;

            const int Size = VerticeSize + UVSize + NormalSize;

            switch (type)
            {
                case AttributeTypes.Vertice:
                    {
                        size = 3;
                        ptrType = VertexAttribPointerType.Float;
                        normalized = false;
                        stride = Size * sizeof(float);
                        offset = 0;

                        break;
                    }

                case AttributeTypes.UV:
                    {
                        size = 2;
                        ptrType = VertexAttribPointerType.Float;
                        normalized = false;
                        stride = Size * sizeof(float);
                        offset = VerticeSize * sizeof(float);

                        break;
                    }

                case AttributeTypes.Normal:
                    {
                        size = 3;
                        ptrType = VertexAttribPointerType.Float;
                        normalized = false;
                        stride = Size * sizeof(float);
                        offset = (VerticeSize + UVSize) * sizeof(float);

                        break;
                    }

                default:
                    {
                        size = 0;
                        ptrType = VertexAttribPointerType.UnsignedByte;
                        normalized = false;
                        stride = 0;
                        offset = 0;

                        Debug.Log("Something went wrong..");

                        break;
                    }
            }

            GL.VertexAttribPointer(data.Location, size, ptrType, normalized, stride, offset);


            return true;
        }

        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int code);

            if (code != (int)All.True)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Debug.LogError($"[OpenGL] Error occurred while compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int code);

            if (code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst linking Program {program} : " + GL.GetProgramInfoLog(program));
            }
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        internal int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }

        private static string LoadSource(string path)
        {
            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(this.Handle);
            GL.UniformMatrix4(this.Uniforms.First(sh => sh.Name == name).Location, true, ref data);
        }

        public override void Delete()
        {
            GL.DeleteProgram(Handle);
            Handle = -1;

            Cache.Remove(this);

            base.Delete();
        }
    }
}
