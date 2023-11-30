using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain_test : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3[] vertex;
    public int[] triangles;
    Mesh flate;
    public Vector3[] vertex_squares;
    public Triangles_Script trig;

    void Start()
    {
        flate = new Mesh();
        GetComponent<MeshFilter>().mesh = flate;
        construct(1000, 10, 10, 10);
    }

    // Update is called once per frame
   public  void construct(int verticies, float smallest_x, float smallest_y, float smallest_z)
    {
        int vertex_amount = verticies;
        float ratio = verticies/(0.6666f);
        int triabgles_amount = (int)ratio;
        vertex = new Vector3[vertex_amount];
        float x = smallest_x, y = smallest_y, z = smallest_z;
        int increase_index = 0, next_ = 0;
        
         for(int i = 0; i< (int)(verticies/10); i++)
         {
             if (increase_index >= (vertex_amount-1))
             {
                 break;
             }
             for (int z_ = 0; z_ < 10; z_++)
             {
                 for (int x_ = 0; x_ < 10; x_++)
                 {
                    if (next_ + 3 > trig.floats.Count)
                    {
                        next_ = 0;
                    }
                    y = trig.floats[next_ + 2];
                
                     vertex[increase_index] = new Vector3(x, y,z);
                     x +=50;
                     increase_index++;
                     next_ += 3;
                     Debug.Log(increase_index);
                   
                 }
                 z -=20;
                 x = smallest_x;
             }
         }
     


        vertex_squares = new Vector3[vertex_amount];

        int increase_i = 0;
        vertex_squares[0] = vertex[0];
        vertex_squares[1] = vertex[10];
        vertex_squares[2] = vertex[11];
        vertex_squares[3] = vertex[1];
        for (int i = 2; i < vertex_squares.Length; i++)
        {
            vertex_squares[i] = vertex[increase_i];
            vertex_squares[i + 1] = vertex[increase_i + 10];
            i++;
            increase_i++;
        }


        Debug.Log("The size of the triangle array should be " + triabgles_amount);
        triangles = new int[triabgles_amount];
        

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        triangles[6] = 3;
        triangles[7] = 2;
        triangles[8] = 4;

        triangles[9] = 3;
        triangles[10] = 4;
        triangles[11] = 5;

        int j = 0;
        int n_ = 3;
        int d = 2;

        for (int i = 12; i<triangles.Length/6; i++)
          {

              triangles[i + j] = d+n_;       
              triangles[i+1+j] = d+n_-1;     
              triangles[i+2+j] = d+n_+1;     

              triangles[i + 3+j] = d+n_; 
              triangles[i + 4 + j] = n_+d+1;  
              triangles[i + 5 + j] = n_+d+2;  

            j += 5;
            n_ += 2;
            
          }
        


        flate.vertices = vertex_squares;
        flate.triangles = triangles;
    }

    

}
