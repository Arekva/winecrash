using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Winecrash.Engine
{
    public sealed class Material : BaseObject
    {
        private Shader _Shader;
        public Shader Shader
        {
            get
            {
                return this._Shader;
            }

            set
            {
                this._Shader = value ?? Shader.ErrorShader;
                BuildDataDictionnary();
            }
        }

        public Material(Shader shader) : base()
        {
            this.Shader = shader;
        }

        private class MaterialData
        {
            public string Name { get; }
            public int Location { get; }
            public object Data { get; set; }
            public ActiveUniformType GLType { get; }

            public MaterialData(string name, int location, object data, ActiveUniformType type)
            {
                this.Name = name;
                this.Data = data;
                this.GLType = type;
                this.Location = location;
            }
        }

        private MaterialData[] _Data;

        internal void Use()
        {
            Shader.Use();

            int texCount = 0;
            for (int i = 0; i < _Data.Length; i++)
            {
                MaterialData data = _Data[i];

                switch (data.GLType) //if texture
                {
                    case ActiveUniformType.Sampler2D:
                        {
                            const string textureEnumText = "Texture";

                            ((Texture)data.Data).Use((TextureUnit)Enum.Parse(typeof(TextureUnit), textureEnumText + texCount));
                            this.SetInt(data.Location, texCount);

                            texCount++;
                            break;
                        }

                    case ActiveUniformType.Double:
                        this.SetDouble(data.Location, (double)data.Data);
                        break;
                    case ActiveUniformType.Float:
                        this.SetFloat(data.Location, (float)data.Data);
                        break;
                    case ActiveUniformType.Int:
                        this.SetInt(data.Location, (int)data.Data);
                        break;
                    case ActiveUniformType.FloatVec2:
                        this.SetVector2(data.Location, (Vector2)data.Data);
                        break;
                    case ActiveUniformType.FloatVec3:
                        this.SetVector3(data.Location, (Vector3)data.Data);
                        break;
                    case ActiveUniformType.FloatMat4:
                        this.SetMatrix4(data.Location, (Matrix4)data.Data);
                        break;
                }
            }
        }

        private void BuildDataDictionnary()
        {
            Shader.ShaderUniformData[] shaderData = this._Shader.Uniforms;
            this._Data = new MaterialData[shaderData.Length];


            for (int i = 0; i < shaderData.Length; i++)
            {
                Type csType = GetCsharpType(shaderData[i].Type);
                if (csType == null)
                {
                    Debug.LogError("Unknown type \"" + shaderData[i].Type + "\", ignoring.");
                    continue;
                }

                if(csType == typeof(Texture))
                {
                    if (Texture.Blank)
                    {
                        _Data[i] = new MaterialData(shaderData[i].Name, shaderData[i].Location, Texture.Blank, shaderData[i].Type);
                    }
                    else
                    {
                        _Data[i] = new MaterialData(shaderData[i].Name, shaderData[i].Location, Activator.CreateInstance(csType), shaderData[i].Type);
                    }


                    continue;
                }



                _Data[i] = new MaterialData(shaderData[i].Name, shaderData[i].Location, Activator.CreateInstance(csType), shaderData[i].Type);
            }
        }

        private Type GetCsharpType(ActiveUniformType gltype)
        {
            return gltype switch
            {
                //ActiveUniformType.Bool => typeof(bool),
                ActiveUniformType.Double => typeof(double),
                ActiveUniformType.Float => typeof(float),
                ActiveUniformType.Int => typeof(int),
                ActiveUniformType.FloatVec3 => typeof(Vector3),
                ActiveUniformType.FloatVec2 => typeof(Vector2),
                ActiveUniformType.Sampler2D => typeof(Texture),
                ActiveUniformType.FloatMat4 => typeof(Matrix4),
                _ => null,
            };
        }

        public override void Delete()
        {
            this.Shader = null;

            base.Delete();
        }

        public void SetData<T>(string name, T data)
        {
            if (this.Deleted) return;

            MaterialData matdata = this._Data.First(d => d.Name == name);
            if (data.GetType() == matdata.Data.GetType())
            {
                //Texture previous = matdata.Data;
                matdata.Data = data;
            }
        }

        public object GetData(string name)
        {
            return this._Data.First(d => d.Name == name).Data;
        }
        public T GetData<T>(string name)
        {
            return (T)GetData(name);
        }

        private void SetInt(int location, int data)
        {
            GL.Uniform1(location, data);
        }

        private void SetFloat(int location, float data)
        {
            GL.Uniform1(location, data);
        }

        private void SetDouble(int location, double data)
        {
            GL.Uniform1(location, data);
        }

        private void SetMatrix4(int location, Matrix4 data)
        {
            GL.UniformMatrix4(location, true, ref data);
        }

        private void SetVector2(int location, Vector2 data)
        {
            GL.Uniform2(location, ref data);
        }

        private void SetVector3(int location, Vector3 data)
        {
            GL.Uniform3(location, ref data);
        }

    }
}
