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
            playerScript.setRun(c == UserInputController.Click.LEFT_DOUBLE);
            playerScript.moveTo(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}