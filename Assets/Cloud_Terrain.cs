using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Globalization;
using UnityEngine.UI;

public class Triangles_Script : MonoBehaviour
{
    // Start is called before the first frame update
    float[] x_y_z = new float[3];
    public List<float[]> index = new List<float[]>();
    public List<float> floats = new List<float>();
    public Vector3[] vertex_data;
    public int[] triangle_constr = new int[6000];
    public Vector2[] uv;

    public GameObject sphere;
    int skip_lines = 3000;
    public int point_amount;
    public Mesh triangles;
    public GameObject ball;
    public Terrain_test terr;

    float time = 0;
    float interval = 1.5f;
    public Text print_triangles;

    public Vector3[] normalized_triangle_vectors;
    int count_normals = 0;
    public int normal_index = 0;
    public int normal_index_for_ball;
    public bool collision_for_ball;
    public int index_triangle_for_ball;
    public int for_height;

    public int[] barysentric_coordinates_ = new int[3];
    public float hoyde_y;
    public float y_for_ball;
    private void Awake()
    {
        //read_files("/Koordinater_xyz.txt");
        read_files("/Coordinates.txt");
        
        // create_terrain();
        sort_floats();  //sorting the list to find the min and max x and y values, useful for making the terrain
       
        Debug.Log(floats[0]);
        
        points_amount("/Coordinates.txt");
        
    }
    void Start()
    {
        triangles = new Mesh();
        GetComponent<MeshFilter>().mesh = triangles;
        create_terrain();
        create_cloud();
        normalized_triangle_vectors = new Vector3[triangles.triangles.Length / 3];
        for (int i = 0; i < triangles.triangles.Length; i++)
     {
            
            normalized_triangle_vectors[count_normals] = normals_of_the_triangles(triangles.triangles[i], triangles.triangles[i + 1], triangles.triangles[i + 2]);
            //Debug.Log("NORMALIZED TRIANGLE " + normalized_triangle_vectors[i]);
            i += 2;
            count_normals++;
        }

       // Debug.Log("Normalized Triangle size " + normalized_triangle_vectors.Length  + "  TRIANGLES SIZE " +triangles.triangles.Length);
    }


    // Update is called once per frame
    void Update()
    {
      if (time > interval)
      {
          bool coll;
          int triangle_check = 0;
            
            normal_index = 0;
       

        for (int i = 0; i < triangles.triangles.Length; i++)
        {
            triangle_check += 1; //used to show the triangle where the ball is placed
            bool check = barys_coord(ball.transform.position, triangles.triangles[i], triangles.triangles[i + 1], triangles.triangles[i + 2]);
            if (check == true)
            {
                triangle_check -= 2;  //used to show the triangle on the screen
                print_triangles.text = triangle_check.ToString();
                coll = collision(ball.transform.position, triangles.triangles[i], normalized_triangle_vectors[normal_index]); //check collisiom
                normal_index_for_ball = normal_index; //used in the ball class
                y_for_ball = hoyde_y; //calculated height in baryc coordinates and is used in the ball class

            }
            i += 2;
            normal_index++;
        }
             time = 0;
         }
         time += Time.deltaTime;
    }

    

    void create_cloud() // funstion that draws point cloud
    {
        for (int i = 0; i < terr.GetComponent<Terrain_test>().vertex_squares.Length; i++) 
        {
            Vector3 pos = terr.GetComponent<Terrain_test>().vertex_squares[i]; //position for points
            GameObject point = Instantiate(sphere, pos, Quaternion.identity); //points themselves
            point.transform.parent = transform; // positionate the point relative to the Unity empty 3D-object called "Cloud Terrain"
            
        }
    }


    void read_files(string file_path)
    {
        try
        {
            string File_Path = Application.dataPath + file_path;

            using (StreamReader reader = new StreamReader(File_Path))
            {
                int line_counter = 0;
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    line_counter++;
                    if (line_counter % skip_lines == 0)
                    {
                        string[] floats_s = line.Split(new char[] { ',', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                        foreach (string float_str in floats_s)
                        {

                            if (float.TryParse(float_str, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out float parse_f))
                            {
                                floats.Add(parse_f);


                            }
                            else
                            {
                                Debug.Log("Failed to parse");
                            }
                        }
                        foreach (float parsed_float in floats)
                        {
                            Debug.Log(parsed_float + " parsed");
                        }

                        point_amount = floats.Count;
                       
                       
                    }
                }
                
            }
        }
        catch (IOException a)
        {
            Debug.Log("Failed to read" + a.Message);
        }

       for (int i = 0; i<floats.Count; i++)
       {
           floats[i] = floats[i]/10000.0f;
           floats[i+1] = floats[i+1]/10000.0f;

            i += 2;
           
        }
        
    }


    void create_terrain()
    {
        
       
        uv = new Vector2[vertex_data.Length]; //we need it to give the terrain a color or a texture
        terr.construct(1000, floats[0], floats[2], floats[1]); //passing floats, min and max values of each x, y and z, and the amount of points
        triangles.vertices = terr.vertex_squares; //passing the data for verticies from Test_terrein to our main terrain
        triangles.triangles = terr.triangles; //the same but for triangle indecies

        
    }



    void sort_floats() //sorting the array of floats (xyz coordinates) relative to x
    {
        int j = 0;  float current = 0; 
        for (int i = 3; i < floats.Count; i++) {
            current = floats[i];
            j = i - 3;

            while (j >= 0 && floats[j] >current)
            {
                floats[j + 3] = floats[j];
                j = j - 3;
            
            }
            floats[j+3] = current;

            i = i + 2;

        }
    }


    void points_amount(string path_file) //draws the amount of points on top of the text file with coordinates
    {
        string File_Path = Application.dataPath + path_file;
        int lines_amount = 0;
        if (File.Exists(File_Path))
        {
            string text_cont = File.ReadAllText(File_Path);
            string[] lines = File.ReadAllLines(File_Path);
            lines_amount = lines.Length;
            string amount_to_write = lines_amount.ToString();
           // Debug.Log("Amount of points " + amount_to_write);
            string new_text = amount_to_write+ Environment.NewLine + text_cont;
            
            using (var write = new StreamWriter(File_Path, false))
            {
                write.WriteLine(new_text);
            }

        }
        else
        {
            Debug.Log("No file exists ");

        }
    }



 bool barys_coord(Vector3 inter_point, int index_1 , int index_2, int index_3) //pass the vertex array indecies here
 {
     Vector3 P1 =triangles.vertices[index_1];
     Vector3 P2 =triangles.vertices[index_2];
        Vector3 P3 = triangles.vertices[index_3];

     Vector2 balls_position = new Vector3(inter_point.x, inter_point.z);
     Vector2 x_1 = new Vector2(P2.x - P1.x, P2.z - P1.z);
     Vector2 x_2 = new Vector2(P3.x - P1.x, P3.z - P1.z);
     float areal_x = cross_product(x_1, x_2);

      //  Vector3 ar_x = Vector3.Cross(x_1, x_2);

     Vector2 u_1 = new Vector2(P2.x - balls_position.x, P2.z - balls_position.y);
     Vector2 u_2 = new Vector2(P3.x - balls_position.x, P3.z - balls_position.y);
 
     float areal_u = cross_product(u_1, u_2);
       // Vector3 ar_u = Vector3.Cross(u_1, u_2);
 
     Vector2 v_1 = new Vector2(P1.x - balls_position.x, P1.z - balls_position.y);
     Vector2 v_2 = new Vector2(P3.x - balls_position.x, P3.z - balls_position.y);
     float areal_v = cross_product(v_1, v_2);
     //Vector3 ar_v = Vector3.Cross(v_1, u_2);


        Vector2 w_1 = new Vector3(P1.x - balls_position.x, P1.z - balls_position.y);
     Vector2 w_2 = new Vector3(P2.x - balls_position.x, P2.z - balls_position.y);
     float areal_w = cross_product(w_1, w_2);
 
     float u = areal_u / areal_x;
     float v = areal_v / areal_x;
     float w = areal_w / areal_x;
     float sum = u + v + w;
      
     //Debug.Log("Floats sum:" + sum);
      //Debug.Log("Floats u v w:" + u + " " + v + " " + w);
 
     bool return_check = false;
     if (u < 0 || v < 0 || w < 0)
     {
         return_check = false;
     }
 
     else if (u >= 0 || v >= 0 || w >= 0)
     {

            float h_u = u * P1.y;
            float h_v = v * P2.y;
            float h_w = w * P3.y;
            hoyde_y = h_u + h_v + h_w;
            if (hoyde_y < 230) //for some reason the heigh goes above the heighest point of the surface, this
            {                  // condition brings the ball back to the surface, the reason of this behaviour is unknown
                hoyde_y = 230.0f;
            }
            else if (hoyde_y > 290)
            {
                hoyde_y = 290.0f;
            }
           // Debug.Log("Floats u v w:" + h_u + " " + h_v + " " + h_w);
           // Debug.Log("Floats sum:" + hoyde_y);

            return_check = true;
     }
     return return_check;
 }


    public float cross_product(Vector2 p1, Vector2 p2)
    {
        float result = (p1.x * p2.y) - (p1.y * p2.x);
        return result;
    }
    public float magnitude(Vector3 vex)
    {
        float result = Mathf.Sqrt(Mathf.Pow(vex.x, 2) + Mathf.Pow(vex.y, 2) + Mathf.Pow(vex.z, 2));
        //Debug.Log("magnitude results: " + result);
        return result;
    }


    Vector3 normals_of_the_triangles(int index_1, int index_2, int index_3)
    {
        Vector3 ver1 = triangles.vertices[index_1];
        Vector3 ver2 = triangles.vertices[index_2];
        Vector3 ver3 = triangles.vertices[index_3];

        Debug.Log("xyz  " + ver1.x + "  " + ver1.y + "  " + ver1.z);
        Debug.Log("xyz  " + ver2.x + "  " + ver2.y + "  " + ver2.z);
        Debug.Log("xyz  " + ver3.x + "  " + ver3.y + "  " + ver3.z);
        Vector3 vector_12 = new Vector3(ver2.x - ver1.x, ver2.y - ver1.y, ver2.z - ver1.z);
        Vector3 vector_13 = new Vector3(ver3.x - ver1.x, ver3.y - ver1.y, ver3.z - ver1.z);

        float x = (vector_12.y * vector_13.z) - (vector_13.y * vector_12.z); //cross product
        float y = -((vector_12.x * vector_13.z) - (vector_13.x * vector_12.z));
        float z = (vector_12.x * vector_13.y) - (vector_13.x * vector_12.y);
        //Debug.Log("z results: " + z);


        Vector3 normal_vector = new Vector3(x, y, z);
        Vector3 normalized_vector = new Vector3(normal_vector.x / magnitude(normal_vector), normal_vector.y / magnitude(normal_vector), normal_vector.z / magnitude(normal_vector));
        return normalized_vector;

    }


    //check the collision between the ball and the terrain
   public bool collision(Vector3 ball_pos, int triangle_vert, Vector3 normalized_vec)
    {
        bool collided;
        Vector3 treangle_point = triangles.vertices[triangle_vert]; //takes the index of vertex of the triangle
        float x_ = ball_pos.x - treangle_point.x;     //calculate the distance
        float y_ = ball_pos.y - treangle_point.y;
        float z_ = ball_pos.z - treangle_point.z;
        Vector3 difference = new Vector3(x_, y_, z_); //the difference between the two points
        float radius = ball.transform.localScale.x * 0.5f; 
        float distance_y = dot_prod(difference, normalized_vec);
        if ((Mathf.Abs(distance_y) <= radius)  || (distance_y>100.0f && distance_y<150.5f)) //we use an additional check to be sure the ball doesnt go under the surface and stays above it 
        {
            collided = true;
            collision_for_ball = true;
        }
        else
        {
            collided = false;
            collision_for_ball = false;
        }
        return collided;
    }

    float dot_prod(Vector3 first, Vector3 second)
    {
        float dot = (first.x * second.x) + (first.y * second.y) + (first.z * second.z);
        return dot;
    }



//  bool colission(Vector3 ball_pos, int index_1 , int index_2, int index_3) //ver_ is for triangle verticies
//  {
//      Vector3 ver_1 = triangles.vertices[index_1];
//      Vector3 ver_2 = triangles.vertices[index_2];
//      Vector3 ver_3 = triangles.vertices[index_3];
//
//      Vector3 normal = Vector3.Cross(ver_2 - ver_1, ver_3 - ver_1).normalized;
//
//      float plane_equation = -Vector3.Dot(normal, ver_1);
//
//      if(Vector3.Dot(normal, ball_pos) + plane_equation < 0)
//      {
//          float ray = -(Vector3.Dot(normal, ball_pos) + plane_equation) / Vector3.Dot(normal, -Vector3.up);
//          Vector3 coll_point = ball_pos + ray * -Vector3.up;
//          bool inside = barys_coord( coll_point,  ver_1,  ver_2,  ver_3);
//          return inside;
//      }
//      return false;
//  }



}
