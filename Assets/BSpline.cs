using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSpline : MonoBehaviour
{
    // Start is called before the first frame update
    public LineRenderer spline; //for b-spline rendering
    public GameObject ball_ref;
    float time_interval = 3.0f;
    float time = 0.0f;
     List<Vector3> points = new List<Vector3>();
    void Start()
    {
        spline = GetComponent<LineRenderer>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
   public void draw_bspline()
    {
        spline.GetComponent<LineRenderer>().positionCount = ball_ref.GetComponent<Ball>().point_render;
        for (int i = 0; i < ball_ref.GetComponent<Ball>().point_render; i++)
        {
            float t = i / (ball_ref.GetComponent<Ball>().point_render - 1); //paramaterization for b-splines t 
            Vector3 point_on_spline = ball_ref.GetComponent<Ball>().calcl_bspline(t);
            spline.SetPosition(i, point_on_spline);
            spline.SetColors(Color.red, Color.red);
            spline.useWorldSpace = true;
        }
    }

}
