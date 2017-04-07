﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurviveTheNight {
    public class Shotgun : Gun {

        private int shellSize = 5;

        void Start() {
            fullAmmo = 4 * shellSize;
            curAmmo = 4 * shellSize;
            reloading = false;
            reloadTime = 2.5f;
            reloadTimeRemain = 0;
            ammoResource = "Buckshot";
            previousShot = DateTime.UtcNow;
            shotDelayMilSec = 0;
        }

        override public void clickType(UserInputController.Click c, Vector2 target) {
            if (c == UserInputController.Click.LEFT_DOWN || c == UserInputController.Click.LEFT_DOUBLE) {
                Vector2[] shell = generateShell(target);
                foreach (Vector2 v in shell) {
                    Fire(v);
                }
            }
        }

        private Vector2[] generateShell(Vector2 target) {
            Vector2[] shell = new Vector2[shellSize];
            shell[0] = target;
            shell[1] = new Vector2(target.x - .2f, target.y + .07f);
            shell[2] = new Vector2(target.x + .2f, target.y - .07f);
            shell[3] = new Vector2(target.x - .07f, target.y - .2f);
            shell[4] = new Vector2(target.x + .07f, target.y + .2f);
            return shell;
        }
    }
}