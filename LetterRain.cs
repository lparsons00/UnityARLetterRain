using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterRain : MonoBehaviour
{


    private List<GameObject> letterPool; // Creating a pool of objects is the most memory efficient way to handle things like this. No new memory will be allocated during runtime.
    private List<GameObject> lettersAlive; //A list of the letters currently alive. 
    private int poolSize; //Poolsize
    public string snow = "0001          PHE   06  000008510694001002+00307003501197006+20057" +
        "00021937 KAP11SCA   06  0000              +00307006102214001+20057" +
        "00031938 EPS 1PHE   04  000028432872002014+00306804317127010+20057" +
        "0004          PHE   07  00010427    002   +00306504421319003+20057" +
        "00051940 GAM31OCT   05  0001    5168   013+00298000648068014+20057" +
        "00061939      OCT   06  00015640    001   +00303101548028002+20056" +
        "00071941 KAP21SCA   06  00023976    001   +00306206113307002+2005 " +
        "00081942 THE 1SCA   06  00024665    001   +00305905353145002+20056" +
        "0009          PHE   06  0003              +00305404651161001+20055    0  3  66 " +
        "0010                07  00031762    001   +00303803030312001+20055";
    public Vector3 startPosMax = new Vector3(10, 10, 3);
    public Vector3 startPosMin = new Vector3(-10, 10, -3);
    public float lettersPerSecond = 10; //How many letters per second. Max is one per frame, see how it's implemented in update.
    public float aliveTime = 2f; //How long each letter should live.

    private float timer = 0; //Timer to keep track of when to add new letters.

    // Use this for initialization
    void Start()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        poolSize = snow.Length;
        letterPool = new List<GameObject>();
        lettersAlive = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject particle = new GameObject(); //Create particle
            particle.AddComponent<TextMesh>().text = snow[i].ToString(); //Let particle letter be the next one in the snow
            particle.AddComponent<Rigidbody>();  //Add a rigidbody to the particle
            particle.transform.parent = transform;  //Set the particle as a child of this gameobject, mostly to keep hierarchy cleaner.
            particle.AddComponent<Letter>();    //Add component Letter to keep track of how long it's been alive. This class is written in the same file, check below.
            particle.SetActive(false); //Turn it off for now. 
            letterPool.Add(particle); //Add to pool
        }

    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > (1 / lettersPerSecond))
        { //Doing it this way means the maximum letters per secons will be one each frame.
            timer = 0;  //reset timer
            ShowNewParticle();
        }


        for (int i = lettersAlive.Count - 1; i >= 0; i--)
        { //Check if any particles should die. Iterating backwards is always best when removing stuff from a list during iteration.
            if (lettersAlive[i].GetComponent<Letter>().timeAlive > aliveTime)
            { //This isn't very good, GetComponent shouldn't be called in an update. Figure out a better way to do this if it impacts performance. Perhaps with event from Letter?
                ReturnLetterToPool(i);
            }
        }
    }

    /// <summary>
    /// Returns the letter to pool.
    /// </summary>
    /// <param name="index">Index in alive-pool</param>
    private void ReturnLetterToPool(int index)
    {
        lettersAlive[index].SetActive(false);
        letterPool.Add(lettersAlive[index]);
        lettersAlive.RemoveAt(index);
    }

    /// <summary>
    /// Shows the new particle. Select random from letterPool, remove from that pool and add to alive pool
    /// </summary>
    private void ShowNewParticle()
    {
        if (letterPool.Count == 0)
        {
            Debug.Log("Pool is empty.");
            return;
        }
        int index = Random.Range(0, letterPool.Count - 1);
        letterPool[index].transform.position = new Vector3(Random.Range(startPosMin.x, startPosMax.x), Random.Range(startPosMin.y, startPosMax.y), Random.Range(startPosMin.z, startPosMax.z)); //Randomize position
        letterPool[index].SetActive(true);
        letterPool[index].GetComponent<Rigidbody>().velocity = Vector3.zero;
        letterPool[index].GetComponent<Letter>().timeAlive = 0;
        lettersAlive.Add(letterPool[index]);
        letterPool.RemoveAt(index);
    }
}

/// <summary>
/// Component added to each particle. Normally this would probably be in a seperate file, but I kept it in the same file for ease of use.
/// Should be pretty self explanatory
/// </summary>
public class Letter : MonoBehaviour
{

    public float timeAlive = 0;
    private Vector3 rotations = new Vector3(3, 12, 50);

    private void OnEnable()
    {

        transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;
        transform.Rotate(rotations * Time.deltaTime);

    }

}