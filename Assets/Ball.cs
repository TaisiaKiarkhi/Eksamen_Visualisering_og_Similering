using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // Start is called before the first frame update
    //for physics calculation
    float gravitation = 9.8f;
    float initial_speed = 0; //the speed of the ball when we drop it
    float speed = 0; // the speed after some time passed
    Vector3 speed_; // vector for speed used to move thr ball in the right direction
    Vector3 initial_velocity = new Vector3(1.0f, 1.0f, 1.0f); //vector for initial speed used in physics calculation (previous vector of speed)
    Vector3 start_position; //initial position of the ball, used in physics calculation of know the right position of the ball
    float mass = 2.0f; // two grams
    //for right movement and collision checking
    public GameObject terrain;
    public  bool collided = false;
    float height; //y coordinates 
    
    //for b-spline calculations
   public int point_render = 10; //controll points for b-spline (amount)
   public Vector3[] contrl_points; 
                                                  
    public GameObject spline_ref;
    float Time_ = 0;
    
    void Start()
    {
       
         
        contrl_points = new Vector3[50];
        
    }


    // Update is called once per frame
    void Update()
    {
        start_position = gameObject.transform.position; //takes current position of the ball (needs for physics calculation)
        int index = terrain.GetComponent<Triangles_Script>().index_triangle_for_ball; //takes the index of the triangle where the ball is placed
        int index_norm = terrain.GetComponent<Triangles_Script>().normal_index_for_ball; //takes the normal vector of the triangle
        //Debug.Log("COLLISION IS " + collided);
        if (collided == false)
        {
            
            collided = terrain.GetComponent<Triangles_Script>().collision(transform.position, terrain.GetComponent<Triangles_Script>().triangles.triangles[index], terrain.GetComponent<Triangles_Script>().normalized_triangle_vectors[index_norm]);
            fall();
        }
   
       else
       {
            
            roll();

            contrl_points[0] = transform.position;

            if (Time_ > 5)
        {
            for (int i = 1; i < 50; i++)
            {
                contrl_points[i] = transform.position;
                if (i > 5)
                {
                    spline_ref.GetComponent<BSpline>().draw_bspline();
                        
                }
                    new WaitForSecondsRealtime(5);
                }
            Time_ = 0.0f;
            
        }
        Time_ += Time.deltaTime;
       }

      



       }

    public void fall()
    {
        
            speed = initial_speed - gravitation * Time.deltaTime; //calculate the speed
            float y = (initial_speed * Time.deltaTime) + gameObject.transform.position.y - ((1 / 2) * gravitation * (Mathf.Pow(Time.deltaTime, 2))); //update y position (height)
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, y, gameObject.transform.position.z); //set new position
            initial_speed = speed; //update initial speed
        
    }

   public void roll()
    {
       
               int index = terrain.GetComponent<Triangles_Script>().normal_index_for_ball; //takes the current triangle indecies
               //Debug.Log("INDEX NORMALS:  " + index);
               Vector3 triangle_normal = terrain.GetComponent<Triangles_Script>().normalized_triangle_vectors[index]; //takes normal vector of the triangle
        
               float a_x = (triangle_normal.x * triangle_normal.y) * gravitation; //calculates acdelaration. Used from the notations 8.4 rolling ball
               float a_y = (Mathf.Pow(triangle_normal.y, 2) - 1) * gravitation;
               float a_z = (triangle_normal.z * triangle_normal.y) * gravitation;
        
               float v_x = initial_velocity.x +(a_x * Time.deltaTime); //calculates the vilocity in x, y and z directions
               float v_y = initial_velocity.y +(a_y * Time.deltaTime);
               float v_z = initial_velocity.z +(a_z * Time.deltaTime);
 
               int index_ = terrain.GetComponent<Triangles_Script>().for_height; //used to calculate the height of the ball when collided with the terrain
               float x = start_position.x + (initial_velocity.x * Time.deltaTime) + (a_x * Mathf.Pow(Time.deltaTime, 2) * 0.5f); //calculates the position for x
               float y = terrain.GetComponent<Triangles_Script>().y_for_ball; //we use the height calculated in barycentric coordinates from the terrain class
               float z = start_position.z + (initial_velocity.z * Time.deltaTime)  + (a_z * Mathf.Pow(Time.deltaTime, 2) * 0.5f); //for z
        
               transform.position = new Vector3(x, y, z); //update position of the ball
               start_position  = new Vector3(x, y, z);  //update start position
               initial_velocity = new Vector3(v_x, v_y, v_z); //set new velocity
               collided = true;
           
        
    }

 


  IEnumerator record_pos()
    {

        
        for(int  i = 0; i<50; i++) //record 3 times
        {
            contrl_points[i] = transform.position;
            if (i >=3)
            {
               // draw_bspline();
            }
          yield return new WaitForSecondsRealtime(5);  //wait 5 seconds;
        }
           
        
    }



    //attempt to do De Boor
  public Vector3 calcl_bspline(float t) //pass the skjøtvektor
   {

        Debug.Log("Size of contrl points " + contrl_points.Length);
        float basis_u = 1.0f - t; // that function which is (1-t)
        float square_basis_u = Mathf.Pow(basis_u, 2);
       // float cubic_basis_u = Mathf.Pow(basis_u, 3);

        float square_param_t = Mathf.Pow(t, 2);
       // float cubic_param_t = Mathf.Pow(t, 3);

         Vector3 point = square_basis_u * contrl_points[0]; //point * (1-u)^2
         point += (2*t*basis_u )* contrl_points[1]; // 2 * t * (1-t) * next point

         point+= square_param_t * contrl_points[2]; // t^2 * next point

        return point;

    }
}
