using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Rain_drop;
    public GameObject terrain;
    GameObject drop;
    
    void Start()
    {
        
    
      
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Instantiate_drops());
    }

    private IEnumerator Instantiate_drops()
    {
       
        for (int i = 0; i<2; i++)
        {
            float x = Random.RandomRange (46.07970f,  496.12470f); //generates the random numbers 
            float z = Random.RandomRange(219.91190f, 699.95960f);
            float y = Random.RandomRange(350.0f, 650.0f);
            Vector3 pos = new Vector3(x, y, z);
             drop = Instantiate(Rain_drop, pos , Quaternion.identity);
            
           
            
        }
        yield return new WaitForSeconds(10); //waits 10 sekonds before creating a new rain drop
        Destroy(drop);
    }
}
