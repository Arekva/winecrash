using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public class Mesh : BaseObject
    {

        public Mesh() : base() {}

        public Mesh(string name) : base(name) { }

        public Vector3F[] Vertices { get; set; }
        public int[] Triangles { get; set; }
        public Vector2F[] UVs { get; set; }
        public Vector4F[] Tangents { get; set; }
        public Vector3F[] Normals { get; set; }

        public static Mesh[] LoadFile(string path, MeshFormats format)
        {
            return format switch
            {
                MeshFormats.Wavefront => LoadWavefront(path),
                MeshFormats.Blender => LoadBlender(path),
                _ => null,
            };
        }

        private static Mesh[] LoadWavefront(string path)
        {
            int lineID = -1;
            try
            {
                List<Mesh> meshes = new List<Mesh>(1);

                using (StreamReader stream = new StreamReader(path, UTF8Encoding.UTF8))
                {
                    string line;

                    Mesh mesh = new Mesh();
                    meshes.Add(mesh);

                    List<Vector3F> vertices = new List<Vector3F>();
                    List<Vector3F> normals = new List<Vector3F>();
                    List<Vector2F> uvs = new List<Vector2F>();

                    bool verticesStarted = false;

                    while ((line = stream.ReadLine()) != null)
                    {
                        lineID++;

                        if (String.IsNullOrWhiteSpace(line)) continue;

                        // don't read line if it's commented
                        if (line[0] == '#') continue;

                        string[] args = line.Split(' ');
                        string action = args[0];

                        // new object
                        if (action == "o")
                        {
                            if(verticesStarted) // if the vertices description has started, there is a new mesh,
                            {                   // otherwise it gives the first object's name; aka don't create new mesh
                                mesh.Vertices = vertices.ToArray();
                                mesh.UVs = uvs.ToArray();
                                mesh.Normals = vertices.ToArray();

                                mesh = new Mesh();
                                meshes.Add(mesh);

                                vertices.Clear();
                                uvs.Clear();
                                normals.Clear();
                            }

                            mesh.Name = args[1];
                        }

                        else if (action == "v") // vertice
                        {
                            verticesStarted = true;

                            vertices.Add(new Vector3F(Single.Parse(args[1]), Single.Parse(args[2]), Single.Parse(args[3])));
                        }

                        else if (action == "vt") // uv
                        {
                            uvs.Add(new Vector2F(Single.Parse(args[1]), Single.Parse(args[2])));
                        }

                        else if (action == "vn") // normals
                        {
                            normals.Add(new Vector3F(Single.Parse(args[1]), Single.Parse(args[2]), Single.Parse(args[3])));
                        }

                        //todo: faces, materials
                    }

                    mesh.Vertices = vertices.ToArray();
                    mesh.UVs = uvs.ToArray();
                    mesh.Normals = vertices.ToArray();
                }

                return meshes.ToArray();
            }
            catch(FileNotFoundException)
            {
                Debug.LogError("Unable to load wavefront at path " + path + " : file not found.");
                return null;
            }
            catch(FormatException)
            {
                Debug.LogError("Error when loading wavefront at path " + path + " : error at line " + lineID);
                return null;
            }
            catch(Exception e)
            {
                Debug.LogError("Error when loading wavefront at path " + path + " : " + e.Message);
                return null;
            }
            
        }

        private static Mesh[] LoadBlender(string path)
        {
            throw new NotImplementedException("*.Blend Blender format is not supported yet.");
        }

        internal override void ForcedDelete()
        {
            this.Delete();
            base.ForcedDelete();
        }
        public override void Delete()
        {
            this.Vertices = null;
            this.Triangles = null;
            this.UVs = null;
            this.Tangents = null;
            this.Normals = null;

            base.Delete();
        }
    }
}
