/*
* Player.cs
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
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

public class Player : MonoBehaviour
{
  [Header("References")]
  public Transform trans;

  [Header("X Bounds")]
  public float minimumX = -70.0f;
  public float maximumX = 70.0f;

  [Header("Y Bounds")]
  public float minimumY = -18.0f;
  public float maximumY = 80.0f;

  [Header("Z Bounds")]
  public float minimumZ = -130.0f;
  public float maximumZ = 70.0f;

  [Header("Movement")]
  [Tooltip("Distance traveled per seconds with the arrow keys.")]
  public float arrowKeySpeed = 80.0f;

  [Tooltip("Multiplier for mouse drag movement. A higher value will result in the camera moving a greater distance when the mouse is moved.")]
  public float mouseDragSensitivity = 2.8f;

  [Tooltip("Amount of smothing applied to camera movement. Should be a value between 0 and 1.")]
  [Range(0.0f, 0.99f)]
  public float movementSmothing = 0.75f;

  private Vector3 targetPosition;

  [Header("Scrolling")]
  [Tooltip("Amount of Y distance the camera moves per mouse scroll increment.")]
  public float scrollSensitivity = 1.6f;

  private enum Mode
  {
    Build,
    Play
  }

  private Mode mode = Mode.Build;

  [Header("Build Mode")]
  [Tooltip("Layer mask for highlighter raycasting. Should inlude the layer of the stage")]
  public LayerMask stageLayerMask;

  [Tooltip("Reference to the Transform of the Highlighter GameObject.")]
  public Transform highlighter;

  [Tooltip("Reference to the Tower Selling Panel.")]
  public RectTransform towerSellingPanel;

  [Tooltip("Reference to the Text component of the Refund Text in the Tower Selling Panel.")]
  public TMP_Text sellRefundText;

  [Tooltip("Reference to the Text component of the current gold text in the bottom-left corner of the UI.")]
  public TMP_Text currentGoldText;

  [Tooltip("The colour to apply to the selected build button.")]
  public Color selectedBuildButtonColor = new Color(0.2f, 0.8f, 0.2f);

  [Tooltip("Reference to the sell button lock panel GameObject.")]
  public GameObject sellButtonLockPanel;
  
  //Mouse position at the last frame.
  private Vector3 lastMousePosition;

  //Current gold the last time we checked.
  private int goldLastFrame;

  private int gold = 50;

  //True if the cursor is over the stage right now, false if not.
  private bool cursorIsOverStage = false;

  //Reference to the Tower prefab selected by the build button.
  private Tower towerPrefabToBuild = null;

  //Refernce to the currently selected build button Image component.
  private UnityEngine.UI.Image selectedBuildButtonImage = null;

  //Currently selected Tower instance, if any.
  private Tower selectedTower = null;

  private Dictionary<Vector3, Tower> towers = new Dictionary<Vector3, Tower>();

  public Transform spawnPoint;
  public Transform leakPoint;

  //Play Mode:
  [Header("Play Mode")]
  [Tooltip("Reference to the Build Button Panel to deactivate it when play mode starts.")]
  public GameObject buildButtonPanel;

  [Tooltip("Reference to the Game Lost Panel.")]
  public GameObject gameLostPanel;

  [Tooltip("Reference to the Text component for the info text in the Gane Lost Panel.")]
  public TMP_Text gameLostPanelInfoText;

  [Tooltip("Reference to the Play Button GameObject to deactivate it in play mode.")]
  public GameObject playButton;

  [Tooltip("Reference to the Enemy Holder Transform.")]
  public Transform enemyHolder;

  [Tooltip("Reference to the ground enemy prefab.")]
  public Enemy groundEnemyPrefab;

  [Tooltip("Reference to the flying enemy prefab.")]
  public Enemy flyingEnemyPrefab;

  [Tooltip("Time in seconds between each enemy spawning.")]
  public float enemySpawnRate = 0.35f;

  [Tooltip("Determines how often flying enemy levels occur. For example if this is set to 4, every 4th level is a flying level.")]
  public int flyingLevelInterval = 4;

  [Tooltip("Number of enemies spawned each level.")]
  public int enemiesPerLevel = 15;

  [Tooltip("Gold given to the player at the end of each level")]
  public int goldRewardPerLevel = 12;

  //The current level
  public static int level = 1;

  //Number of enemies spawned so far for this level
  private int enemiesSpawnedThisLevel = 0;

  //Player's number of remaining lives; once it hits 0, the game is over;
  public static int remainingLives = 40;


  void ArrowKeyMovement()
  {
    //If up arrow is held,
    if(Input.GetKey(KeyCode.UpArrow))
    {
      //...add to target Z position:
      targetPosition.z += arrowKeySpeed * Time.deltaTime;
    }
    //Otherwise, if down arrow is held,
    else if(Input.GetKey(KeyCode.DownArrow))
    {
      //...subtract from target Z position:
      targetPosition.z -= arrowKeySpeed * Time.deltaTime;
    }
    //If right arrow is held,
    if(Input.GetKey(KeyCode.RightArrow))
    {
      //...add to target X position:
      targetPosition.x += arrowKeySpeed * Time.deltaTime;
    }
    //Otherwise, if left arrow is held,
    else if (Input.GetKey(KeyCode.LeftArrow))
    {
      //..subtract from target X position:
      targetPosition.x -= arrowKeySpeed * Time.deltaTime;
    }
  }

  void MouseDragMovement()
  {
    //If the right mouse button is held,
    if(Input.GetMouseButton(1))
    {
      //Get the movment amount this frame:
      Vector3 movement = new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y")) * mouseDragSensitivity;
      //If there is any movement,
      if(movement != Vector3.zero)
      {
        //..apply it to the targetPosition:
        targetPosition += movement;
      }
    }
  }

  void Zooming()
  {
    //Get thescroll delta Y value and flip it:
    float scrollDelta = -Input.mouseScrollDelta.y;

    //If there was any delta,
    if(scrollDelta != 0)
    {
      //..apply it to the Y position:
      targetPosition.y += scrollDelta * scrollSensitivity;
    }
  }

  void MoveTowardsTarget()
  {
    //Clamp the target position to the bounds variables:
    targetPosition.x = Mathf.Clamp(targetPosition.x, minimumX, maximumX);
    targetPosition.y = Mathf.Clamp(targetPosition.y, minimumY, maximumY);
    targetPosition.z = Mathf.Clamp(targetPosition.z, minimumZ, maximumZ);
    
    //Move if we aren't already at the target position:
    if(trans.position != targetPosition)
    {
      trans.position = Vector3.Lerp(trans.position, targetPosition, 1 - movementSmothing);
    }
  }

  void PositionHighlighter()
  {
    //If the mouse position this frame is different than last frame:
    if(Input.mousePosition != lastMousePosition)
    {
      //Get a ray at the mouse position, shotting out of the camera:
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit; //Information on what was hit will be stored here

      //Cast the ray and check if it hit anything, using our layer mask:
      if(Physics.Raycast(ray, out hit, Mathf.Infinity, stageLayerMask.value))
      {
        //If it did hit something, use hit.point to get the location it hit:
        Vector3 point = hit.point;

        //Round the X and Z values to multiples of 10:
        point.x = Mathf.Round(hit.point.x * 0.1f) * 10;
        point.z = Mathf.Round(hit.point.z * 0.1f) * 10;

        //Clamp Z between -80 and 80 to prevent sticking over the edge of the stage:
        point.z = Mathf.Clamp(point.z, -80, 80);

        //Ensure Y is always 0:
        point.y = 0.2f;

        //Make sure the highlighter is active (visible) and set its position:
        highlighter.position = point;
        highlighter.gameObject.SetActive(true);
        cursorIsOverStage = true;
      }
      else //If the ray didn't hit anything,
      {
        //...mark cursorIsOverStage as false:
        cursorIsOverStage = false;

        //Deactivate the highlighter GameObject so it no longer shows:
        highlighter.gameObject.SetActive(false);
      }
    }
    //Make sure we keep track of the mouse position this frame:
    lastMousePosition = Input.mousePosition;
  }

  void BuildTower(Tower prefab, Vector3 position)
  {
    //Instaniate the tower at the given location and place it in the Dictionary:
    towers[position] = Instantiate<Tower>(prefab, position, Quaternion.identity);

    //Decrease player gold:
    gold -= towerPrefabToBuild.goldCost;

    //Update the path through the maze:
    UpdateEnemyPath();
  }

  private void PositionSellPanel()
  {
    //If there is a selected tower:
    if(selectedTower != null)
    {
      //Convert tower world position, moved forward by 8 units, to screen space:
      var screenPosition = Camera.main.WorldToScreenPoint(selectedTower.transform.position + Vector3.forward * 8);

      //Apply the position to the tower selling panel:
      towerSellingPanel.position = screenPosition;
    }
  }

  private void UpdateCurrentGold()
  {
    //If the gold has changed since last frame:
    if(gold != goldLastFrame)
    {
      //Update the text to match:
      currentGoldText.text = gold + " gold";
    }
    //Keep track of the gold value each frame:
    goldLastFrame = gold;
  }

  public void DeselectTower()
  {
    //Null selected tower and hide the sell tower panel:
    selectedTower = null;
    towerSellingPanel.gameObject.SetActive(false);
  }

  private void DeselectBuildButton()
  {
    //Null the tower prefab to build, if there is one:
    towerPrefabToBuild = null;

    //Reset the colour of the selected build button, if there is one:
    if(selectedBuildButtonImage != null)
    {
      selectedBuildButtonImage.color = Color.white;
      selectedBuildButtonImage = null;
    }
  }

  private void PerformPathfinding()
  {
    //Payhfind from spawn point to leak point, storing the result in GroundEnemy.path:
    NavMesh.CalculatePath(spawnPoint.position, leakPoint.position, NavMesh.AllAreas, GroundEnemy.path);
    if(GroundEnemy.path.status == NavMeshPathStatus.PathComplete)
    {
      //If the path was successfully found, make sure the lock panel is inactive:
      sellButtonLockPanel.SetActive(false);
    }
    else
    {
      //Activate the lock panel:
      sellButtonLockPanel.SetActive(true);
    }
  }

  private void UpdateEnemyPath()
  {
    Invoke("PerformPathfinding", 0.1f);
  }

  void SellTower(Tower tower)
  {
    //Since it's not going to exist in a bit, deselct the tower:
    DeselectTower();

    //Refund the player:
    gold += Mathf.CeilToInt(tower.goldCost * tower.refundFactor);

    //Remove the tower from the dictionary using its position:
    towers.Remove(tower.transform.position);

    //Destroy the tower GameObject:
    Destroy(tower.gameObject);

    //Refresh pathfinding:
    UpdateEnemyPath();
  }

  public void OnSellTowerButtonClicked()
  {
    //If there is a selected tower,
    if(selectedTower != null)
    {
      //Sell it:
      SellTower(selectedTower);
    }
  }
  
  public void OnBuildButtonClicked(Tower associatedTower)
  {
    //Set the prefab to build:
    towerPrefabToBuild = associatedTower;

    //Clear selected tower (if any)
    DeselectTower();
  }

  public void SetSelectedBuildButton(UnityEngine.UI.Image clickedButtonImage)
  {
    //Keep a reference to the Button that was clicked:
    selectedBuildButtonImage = clickedButtonImage;

    //Set the colour of the clicked button:
    clickedButtonImage.color = selectedBuildButtonColor;
  }

  void OnStageClick()
  {
    //If a build button is selected:
    if(towerPrefabToBuild != null)
    {
      //If there is no tower in that slot and we have enough gold to build the selected tower:
      if(!towers.ContainsKey(highlighter.position) && gold >= towerPrefabToBuild.goldCost)
      {
        BuildTower(towerPrefabToBuild, highlighter.position);
      }
    }
    //If no build button is selected:
    else
    {
      //Check if a tower is at the current highlighter position:
      if(towers.ContainsKey(highlighter.position))
      {
        //Set the selected tower to this one:
        selectedTower = towers[highlighter.position];

        //Update the refund text:
        sellRefundText.text = "for " + Mathf.CeilToInt(selectedTower.goldCost * selectedTower.refundFactor) + " gold";

        //Make sure the sell tower UI panel is active so it shows:
        towerSellingPanel.gameObject.SetActive(true);
      }
    }
  }

  private void GoToPlayMode()
  {
    mode = Mode.Play;

    //Deactivate build mode button panel and play button:
    buildButtonPanel.SetActive(false);
    playButton.SetActive(false);

    //Deactivate highlighter
    highlighter.gameObject.SetActive(false);
  }

  private void GoToBuildMode()
  {
    mode = Mode.Build;

    //Activate build button panel and play button:
    buildButtonPanel.SetActive(true);
    playButton.SetActive(true);

    //Reset enemies spawned:
    enemiesSpawnedThisLevel = 0;

    //Increase level:
    level += 1;
    gold += goldRewardPerLevel;
  }

  public void StartLevel()
  {
    //Switch to play mode:
    GoToPlayMode();

    //Repeatedly invoke SpawnEnemy:
    InvokeRepeating("SpawnEnemy", 0.5f, enemySpawnRate);
  }

  void BuildModeLogic()
  {
    PositionHighlighter();
    PositionSellPanel();
    UpdateCurrentGold();

    //If the left mouse button is clicked while the cursor is over the stage:
    if(cursorIsOverStage && Input.GetMouseButtonDown(0))
    {
      OnStageClick();
    }

    //If Escape is pressed:
    if(Input.GetKeyDown(KeyCode.Escape))
    {
      DeselectTower();
      DeselectBuildButton();
    }
  }

  void PlayModeLogic()
  {
    //If no enemies are left and all enemies have already spawned
    if(enemyHolder.childCount == 0 && enemiesSpawnedThisLevel >= enemiesPerLevel)
    {
      //Return to build mode if we haven't lost yet:
      if(remainingLives > 0)
      {
        GoToBuildMode();
      }
      else
      {
        //Update game lost panel text wuth information:
        gameLostPanelInfoText.text = "You had " + remainingLives + "lives by the end and made it to level " + level + ".";

        //Activate the game lost panel:
        gameLostPanel.SetActive(true);
      }
    }
  }

  private void SpawnEnemy()
  {
    Enemy enemy = null;

    //If this is a flying flying level
    if(level % flyingLevelInterval == 0)
    {
      enemy = Instantiate(flyingEnemyPrefab, spawnPoint.position + (Vector3.up * 18), Quaternion.LookRotation(Vector3.back));
    }
    else //If it's a ground level
    {
      enemy = Instantiate(groundEnemyPrefab, spawnPoint.position, Quaternion.LookRotation(Vector3.back));
    }

    //Parent enemy to the enemy holder:
    enemy.trans.SetParent(enemyHolder);

    //Count that we spawned the enemy:
    enemiesSpawnedThisLevel += 1;

    //Stop invoking if we've spawned all enemies:
    if(enemiesSpawnedThisLevel >= enemiesPerLevel)
    {
      CancelInvoke("SpawnEnemy");
    }
  }

  //Events:

  // Start is called before the first frame update
  void Start()
  {
    targetPosition = trans.position;
    GroundEnemy.path = new NavMeshPath();
    UpdateEnemyPath();
  }

  // Update is called once per frame
  void Update()
  {
    ArrowKeyMovement();
    MouseDragMovement();
    Zooming();
    MoveTowardsTarget();

    //Run build mode logic if we're in build mode:
    if(mode == Mode.Build)
    {
      BuildModeLogic();
    }
    else
    {
      PlayModeLogic();
    }
  }
}
