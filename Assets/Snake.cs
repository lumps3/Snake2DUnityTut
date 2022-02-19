using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    public int xSize, ySize; 
    public GameObject block; 

    GameObject head; 
    public Sprite headMaterial, tailMaterial, border, inner, point; 
    List<GameObject> tail; 

    Vector2 dir;

    public Text points;
    // Start is called before the first frame update
    void Start()
    {
        timeBetweenMovements = 0.5f;
        dir = Vector2.right;
        createGrid();
        createInnerTiles();
        createPlayer(); 
        spawnFood(); 
        block.SetActive(false);
        isAlive = true;
    }

    private void createInnerTiles(){
        for(int x = -xSize/2+1; x < xSize/2; x++){
        
            for(int y = -ySize/2+1; y < ySize/2; y++){
                GameObject innerTile = Instantiate(block) as GameObject;
                innerTile.GetComponent<Transform>().position = new Vector3(x, y, 1); 
                innerTile.GetComponent<SpriteRenderer>().sprite = inner;
            }
        }

    }



    private Vector2 getRandomPos(){
        return new Vector2(Random.Range(-xSize/2+1, xSize/2), Random.Range(-ySize/2+1, ySize/2)); 
    }

    private bool containedInSnake(Vector2 spawnPos){
        bool isInHead = spawnPos.x == head.transform.position.x && spawnPos.y == head.transform.position.y;
        bool isInTail = false; 
        foreach (var item in tail)
        {
            if(item.transform.position.x == spawnPos.x && item.transform.position.y == spawnPos.y){
                isInTail = true; 
            }
        }
        return isInHead || isInTail;
    }
    GameObject food;
    bool isAlive;

    private void spawnFood(){
        Vector2 spawnPos = getRandomPos();
        while(containedInSnake(spawnPos)){
            spawnPos = getRandomPos();
        }
        food = Instantiate(block);
        food.GetComponent<SpriteRenderer>().sprite = point;
        food.transform.position = new Vector3(spawnPos.x, spawnPos.y, 0);
        food.SetActive(true);
    }

    private void createPlayer(){
        head = Instantiate(block) as GameObject; 
        head.GetComponent<SpriteRenderer>().sprite = headMaterial;
        head.transform.name = "Player";
        tail = new List<GameObject>(); 
    }

    private void createGrid(){
        for(int x = 0; x <= xSize; x++){
            GameObject borderBottom = Instantiate(block) as GameObject; 
            borderBottom.GetComponent<Transform>().position = new Vector3(x-xSize/2, -ySize/2, 1);

            GameObject borderTop = Instantiate(block) as GameObject; 
            borderTop.GetComponent<Transform>().position = new Vector3(x-xSize/2, ySize-ySize/2, 1);
        }

        for(int y = 0; y <= ySize; y++){
            GameObject borderRight = Instantiate(block) as GameObject;
            borderRight.GetComponent<Transform>().position = new Vector3(-xSize/2, y-(ySize/2), 1); 

            GameObject borderLeft = Instantiate(block) as GameObject;
            borderLeft.GetComponent<Transform>().position = new Vector3(xSize-(xSize/2), y-(ySize/2), 1); 
        }
    }

    float passedTime, timeBetweenMovements;

    public GameObject gameOverUI;

    private void gameOver(){
        isAlive = false; 
        gameOverUI.SetActive(true); 
    }

    public void restart(){
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKey(KeyCode.DownArrow)){
            dir = Vector2.down;
        } else if(Input.GetKey(KeyCode.UpArrow)){
            dir = Vector2.up; 
        } else if(Input.GetKey(KeyCode.RightArrow)){
            dir = Vector2.right;
        } else if(Input.GetKey(KeyCode.LeftArrow)){
            dir = Vector2.left;
        }

        passedTime += Time.deltaTime;
        if(timeBetweenMovements < passedTime && isAlive){
            passedTime = 0;
            // Move
            Vector3 newPosition = head.GetComponent<Transform>().position + new Vector3(dir.x, dir.y, 0);

            // Check if collides with border
            if(newPosition.x >= xSize/2
            || newPosition.x <= -xSize/2
            || newPosition.y >= ySize/2
            || newPosition.y <= -ySize/2){
                gameOver();
            }

            // check if collides with any tail tile
            foreach (var item in tail)
            {
                if(item.transform.position == newPosition){
                    gameOver();
                }
            }
            if(newPosition.x == food.transform.position.x && newPosition.y == food.transform.position.y){
                GameObject newTile = Instantiate(block);
                newTile.SetActive(true);
                newTile.transform.position = food.transform.position;
                DestroyImmediate(food);
                head.GetComponent<SpriteRenderer>().sprite = tailMaterial;
                tail.Add(head); 
                head = newTile;
                head.GetComponent<SpriteRenderer>().sprite = headMaterial;
                spawnFood();
                points.text = "Points: " + tail.Count;
            } else {
                if(tail.Count == 0){
                    head.transform.position = newPosition;
                } else {
                    head.GetComponent<SpriteRenderer>().sprite = tailMaterial;
                    tail.Add(head); 
                    head = tail[0];
                    head.GetComponent<SpriteRenderer>().sprite = headMaterial;
                    tail.RemoveAt(0);
                    head.transform.position = newPosition;
                }
            }

        }
        
    }
}
