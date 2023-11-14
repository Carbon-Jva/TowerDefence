/*
* ArcingProjectile.cs
* Description of the content and purpose of the file.
*
* Copyright (c) 2023 Jimmy Vall
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcingProjectile : Projectile
{
  public Transform trans;
  [Tooltip("Layer mask to use when detecting enemies the explosion will affect.")]
  public LayerMask enemyLayerMask;

  [Tooltip("Radius of the explosion.")]
  public float explosionRadius = 25;

  [Tooltip("Curve that should go from value 0 to 1 over 1 second. Defines the curve of the projectile.")]
  public AnimationCurve curve;

  //Position we're aiming to travel to. Will always have a Y value of 0.
  private Vector3 targetPosition;

  //Our position when we spawned.
  private Vector3 initialPosition;

  //Total distance we'll travel from initial position to target position, not counting the Y axis.
  private float xzDistanceToTravel;

  //Time.time at which we spawned.
  private float spawnTime;


  private float FractionOfDistanceTraveled
  {
    get
    {
      float timeSinceSpawn = Time.time - spawnTime;
      float timeToReachDestination = xzDistanceToTravel / speed;

      return timeSinceSpawn / timeToReachDestination;
    }
  }

  protected override void OnSetup()
  {
    //Set initial position to our current position, and target position to the target enemy position:
    initialPosition = trans.position;
    targetPosition = targetEnemy.trans.position;

    //Make sure the target position is always at a Y of 0:
    targetPosition.y = 0;

    //Calculate the total distance we'll need to travel on the X and Z axes:
    xzDistanceToTravel = Vector3.Distance(new Vector3(trans.position.x, targetPosition.y, trans.position.z), targetPosition);

    //Mark the Time.time we spawned at:
    spawnTime = Time.time;
  }
  
  private void Explode()
  {
    Collider[] enemyColliders = Physics.OverlapSphere(trans.position, explosionRadius, enemyLayerMask.value);

    //Loop through enemy colliders:
    for(int i = 0; i < enemyColliders.Length; i++)
    {
      //Get Enemy script component:
      var enemy = enemyColliders[i].GetComponent<Enemy>();

      //If we found an Enemy component:
      if(enemy != null)
      {
        float distToEnemy = Vector3.Distance(trans.position, enemy.trans.position);
        float damageToDeal = damage * (1 - Mathf.Clamp(distToEnemy / explosionRadius, 0f, 1f));
        enemy.TakeDamage(damageToDeal);
      }
    }
    Destroy(gameObject);
  }

  // Start is called before the first frame update
  /*void Start()
  {
    
  }*/

  // Update is called once per frame
  void Update()
  {
    //First, we'll move along the X and Z axes.
    //Get tge current position and zero out the Y axis:
    Vector3 currentPosition = trans.position;
    currentPosition.y = 0;

    //Move the current position towards the target position by 'speed' per second:
    currentPosition = Vector3.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);

    //Now set the Y axis of currentposition:
    currentPosition.y = Mathf.Lerp(initialPosition.y, targetPosition.y, curve.Evaluate(FractionOfDistanceTraveled));

    //Apply the position to our Transform:
    trans.position = currentPosition;

    //Explode if we've reached the target position:
    if(currentPosition == targetPosition)
    {
      Explode();
    }
  }
}
