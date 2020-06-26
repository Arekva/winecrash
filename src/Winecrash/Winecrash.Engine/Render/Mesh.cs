using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Diagnostics;

using OpenTK.Graphics.OpenGL4;

namespace Winecrash.Engine
{
    public class Mesh : BaseObject
    {
        public Mesh() : base() {}

        public Mesh(string name) : base(name) { }

        public Mesh(Mesh original) : base()
        {
            this.Vertices = original.Vertices;
            this.Triangles = original.Triangles;
            this.UVs = original.UVs;
            this.Tangents = original.Tangents;
            this.Normals = original.Normals;
        }

        public Vector3F[] Vertices { get; set; }
        public UInt32[] Triangles { get; set; }
        public Vector2F[] UVs { get; set; }
        public Vector4F[] Tangents { get; set; }
        public Vector3F[] Normals { get; set; }

        internal int ModelHash
        {
            get
            {
                var hashCode = 1397521031;
                hashCode = hashCode * -1521134295 + base.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<Vector3F[]>.Default.GetHashCode(Vertices);
                hashCode = hashCode * -1521134295 + EqualityComparer<uint[]>.Default.GetHashCode(Triangles);
                hashCode = hashCode * -1521134295 + EqualityComparer<Vector2F[]>.Default.GetHashCode(UVs);
                hashCode = hashCode * -1521134295 + EqualityComparer<Vector4F[]>.Default.GetHashCode(Tangents);
                hashCode = hashCode * -1521134295 + EqualityComparer<Vector3F[]>.Default.GetHashCode(Normals);
                return hashCode;
            }
        }

        private readonly float[] _Vertices =
        {
            // Position         Texture coordinates
             0.5f,  0.5f, -0.5f, 1.0f, 1.0f, // top right     - 0
             0.5f, -0.5f, -0.5f, 1.0f, 0.0f, // bottom right  - 1
            -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, // bottom left   - 2
            -0.5f,  0.5f, -0.5f, 0.0f, 1.0f  // top left      - 3
        };

        internal float[] cube_vertices = {
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
};

        internal readonly uint[] _Indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        internal float[] Vertex
        {
            get
            {
                float[] array = new float[this.Vertices.Length * 3 + this.UVs.Length * 2];

                for (int vert = 0; vert < this.Vertices.Length; vert++)
                {
                    Vector3F vertice = this.Vertices[vert];

                    array[vert * 3] = vertice.X;
                    array[vert * 3 + 1] = vertice.Y;
                    array[vert * 2 + 2] = vertice.Z;

                    if (vert >= UVs.Length)
                    {
                        array[vert * 3 + 3] = array[vert * 3 + 4] = 0.0F;
                    }

                    else
                    {
                        Vector2F uv = this.UVs[vert];
                        array[vert * 3 + 3] = uv.X;
                        array[vert * 3 + 4] = uv.Y;
                    }
                }

                return array;
            }
        }

        internal uint[] Indices
        {
            get
            {
                return Triangles;
            }
        }

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
                    List<uint> triangles = new List<uint>();

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
                                mesh.Triangles = triangles.ToArray();

                                mesh = new Mesh();
                                meshes.Add(mesh);

                                vertices.Clear();
                                uvs.Clear();
                                normals.Clear();
                                triangles.Clear();
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

                        else if (action == "f") // faces
                        {
                            //if polygon => 4 vertices, if triangle => 3 vertices 

                            bool polygon = args.Length - 1 == 4;

                            uint[] faceVertices = new uint[polygon ? 4 : 3];

                            for (int i = 0; i < faceVertices.Length; i++) // extracting vertices
                            {
                                faceVertices[i] = UInt32.Parse(args[i + 1][0].ToString());
                            }

                            triangles.Add(faceVertices[0] - 1);
                            triangles.Add(faceVertices[1] - 1);
                            triangles.Add(faceVertices[2] - 1);

                            if (polygon)
                            {
                                triangles.Add(faceVertices[0] - 1);
                                triangles.Add(faceVertices[2] - 1);
                                triangles.Add(faceVertices[3] - 1);
                            }


                        }

                        //todo: faces, materials
                    }

                    mesh.Vertices = vertices.ToArray();
                    mesh.UVs = uvs.ToArray();
                    mesh.Normals = vertices.ToArray();
                    mesh.Triangles = triangles.ToArray();
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
