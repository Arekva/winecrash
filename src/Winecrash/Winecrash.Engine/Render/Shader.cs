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
    public class Shader : BaseObject
    {
        private int Handle = -1;

        private readonly Dictionary<string, int> _UniformLocations;

        public Shader(string vertexPath, string fragmentPath) : base()
        {
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

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int numberOfUniforms);

            _UniformLocations = new Dictionary<string, int>(numberOfUniforms);

            for (int i = 0; i < numberOfUniforms; i++)
            {
                string key = GL.GetActiveUniform(Handle, i, out _, out _);

                int location = GL.GetUniformLocation(Handle, key);

                _UniformLocations.Add(key, location);
            }

        }

        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int code);

            if(code != (int)All.True)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int code);

            if(code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst linking Program {program} : " + GL.GetProgramInfoLog(program));
            }
        }

        internal void Use()
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

        public override void Delete()
        {
            GL.DeleteProgram(Handle);
            Handle = -1;

            base.Delete();
        }

        public void SetInt(string name, int data)
        {
            if (this.Deleted) return;

            GL.UseProgram(Handle);
            GL.Uniform1(_UniformLocations[name], data);
        }

        public void SetFloat(string name, float data)
        {
            if (this.Deleted) return;

            GL.UseProgram(Handle);
            GL.Uniform1(_UniformLocations[name], data);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            if (this.Deleted) return;

            GL.UseProgram(Handle);
            GL.UniformMatrix4(_UniformLocations[name], true, ref data);
        }

        public void SetVector3(string name, Vector3F data)
        {
            if (this.Deleted) return;

            GL.UseProgram(Handle);
            GL.Uniform3(_UniformLocations[name], new Vector3(data.X, data.Y, data.Z));
        }
    }
}
