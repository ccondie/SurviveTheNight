﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurviveTheNight {

    public class MapController : MonoBehaviour {

        GameObject player;
        Player playerScript;

        // Use this for initialization
        void Start() {
            player = GameObject.Find("Player");
            playerScript = (Player) player.GetComponent("Player");
        }

        // Update is called once per frame
        void Update() {

        }

        //Determine if user clicked on map
        public bool clickHit() {
            //TODO: implement
            return true;
        }

        public void processClick(UserInputController.Click c) {
            Debug.Log(c);
            if (c >= UserInputController.Click.RIGHT_DOWN) {
                //MOVEMENT
                playerScript.setRun(c == UserInputController.Click.RIGHT_DOUBLE);
                if (c == UserInputController.Click.RIGHT_DOWN || c == UserInputController.Click.RIGHT_DOUBLE) {
                    playerScript.moveTo(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            } else {
                //ATTACK
            }
        }
    }
}
