using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    int health = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Send Event OnGameEnter
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDamaged(int damage){
        health -= damage;
        Debug.Log($"{this} is taking {damage} damage : {health}");

        if(health <= 0 ){
            // Send event OnDeath
        }
    }
}
