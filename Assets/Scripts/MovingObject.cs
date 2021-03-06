﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurviveTheNight {

	public abstract class MovingObject : MonoBehaviour {

        // moveTime - smaller is slower, larger is faster
        public float moveTime = 2.0f;
		public LayerMask[] blockingLayer;
		protected Animator animator;

		public bool isMoving = false;
        protected bool navigatingPath = false;
        protected Path path;
		protected Vector2 dest;
        protected IEnumerator coroutine;

		protected BoxCollider2D boxCollider;
		protected Rigidbody2D rb2D;
		protected float scale = 0.6f;

		public bool isDead;

		// Use this for initialization
		protected virtual void Start () {
			isDead = false;
			animator = GetComponent<Animator>();
			boxCollider = GetComponent<BoxCollider2D> ();
			rb2D = GetComponent<Rigidbody2D> ();
		}

		private RaycastHit2D LineCastCheck(Vector3 end) {
			RaycastHit2D hit = new RaycastHit2D();
			Vector2 start = transform.position;
			boxCollider.enabled = false;
			for(int i = 0; i < blockingLayer.Length && hit.transform == null; i++)
				hit = Physics2D.Linecast (start, end, blockingLayer[i]);
			boxCollider.enabled = true;
			return hit;
		}

        protected bool Move(int xDir, int yDir, out RaycastHit2D hit) {
			Vector2 end = (Vector2)transform.position + new Vector2 (xDir*scale, yDir*scale);
			hit = LineCastCheck (end);
			if (hit.transform == null) {
				dest = end;
                coroutine = SmoothMovement(end);
                StartCoroutine(coroutine);
				return true;
			}
			return false;
		}

		protected IEnumerator SmoothMovement (Vector3 end) {
			float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            isMoving = true;
			if(!isDead) defineAnimationState(end);
			while (!isDead && sqrRemainingDistance > float.Epsilon && hasLineOfSight(end)) {
				isMoving = true;
				Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, moveTime * Time.deltaTime);
				rb2D.MovePosition (newPosition);
				sqrRemainingDistance = (transform.position - end).sqrMagnitude;
				yield return null;
			}
            isMoving = false;
			if (!isDead) {
				animator.SetTrigger ("stop");
				//Debug.Log ("stop");
			}
		}

        protected void ContinueAStar() {
            Vector2 nextStep = path.calcNextStep(transform.position, boxCollider, blockingLayer);
            if (path.blocked) {
                navigatingPath = false;
                path = null;
                isMoving = false;
                return;
            } else {
                navigatingPath = true;
				dest = nextStep;
                coroutine = SmoothMovement(nextStep);
                StartCoroutine(coroutine);
            }

            if (nextStep == path.steps[0]) {
                //arrived!
                navigatingPath = false;
                path = null;
                //isMoving = false;
            }
        }

        protected virtual void AttemptMoveAStar<T>(Vector2 target) {
            RaycastHit2D hit = LineCastCheck (target);

			if (isDead)
				return;

            if (hit.transform == null) {
                //there's a straight path
                //Debug.Log("Straight path found to target");
				dest = target;
                coroutine = SmoothMovement(target);
                StartCoroutine(coroutine);
            } else {
                //try A* magic
                CalculatePathAStar(target);
            }
        }

        private void CalculatePathAStar(Vector2 target) {
            AStar algorithm = new SurviveTheNight.AStar(transform.position, target, scale);
            path = algorithm.calculatePath();
            Vector2 firstStep = path.calcNextStep(transform.position, boxCollider, blockingLayer);
            if (path.blocked) {
                navigatingPath = false;
                path = null;
                isMoving = false;
                return;
            } else {
                navigatingPath = true;
				dest = firstStep;
                coroutine = SmoothMovement(firstStep);
                StartCoroutine(coroutine);
            }

            if (firstStep == path.steps[0]) {
                //arrived!
                navigatingPath = false;
                path = null;
                //isMoving = false;
            }
        }
        
        protected void defineAnimationState(Vector2 target) {

			//Debug.Log (this.GetType() is SurviveTheNight.Player);

            int xDir = BoardManager.worldToTile(target.x) - BoardManager.worldToTile(this.transform.position.x);
            int yDir = BoardManager.worldToTile(target.y) - BoardManager.worldToTile(this.transform.position.y);

            int absX = Mathf.Abs(xDir);
            int absY = Mathf.Abs(yDir);

            // Define animation state
            string state = null;
            if (yDir > 0 && yDir > absX << 1)
                state = "walk_north";
            else if (yDir < 0 && absY > absX << 1)
                state = "walk_south";
            else if (xDir < 0 && absX > absY << 1)
                state = "walk_west";
            else if (xDir > 0 && xDir > absY << 1)
                state = "walk_east";
            else if (xDir < 0 && yDir > 0)
                state = "walk_northwest";
            else if (xDir > 0 && yDir > 0)
                state = "walk_northeast";
            else if (xDir < 0 && yDir < 0)
                state = "walk_southwest";
            else if (xDir > 0 && yDir < 0)
                state = "walk_southeast";

            if (state != null) {                
                if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("pm_" + state) || (animator.GetCurrentAnimatorStateInfo(0).IsName("z_" + state))) && !isDead) {
					animator.SetTrigger(state);
					//Debug.Log (state);
                }
                //else if the moving object already has that animation state, don't just reset it
            }
        }

        protected bool surrounded() {
            RaycastHit2D hit;

            Vector2 start = transform.position;
            Vector2 target = new Vector2(start.x, start.y + scale);
			hit = LineCastCheck (target);
            if (hit.transform == null) {
                //there's a straight path
                return false;
            }

            target = new Vector2(start.x, start.y - scale);
			hit = LineCastCheck (target);
            if (hit.transform == null) {
                //there's a straight path
                return false;
            }

            target = new Vector2(start.x + scale, start.y);
			hit = LineCastCheck (target);
            if (hit.transform == null) {
                //there's a straight path
                return false;
            }

            target = new Vector2(start.x - scale, start.y);
			hit = LineCastCheck (target);
            if (hit.transform == null) {
                //there's a straight path
                return false;
            }

            return true;
        }

        protected bool hasLineOfSight(Vector2 target) {
            RaycastHit2D hit = LineCastCheck (target);

            if (hit.transform == null) {
                //there's a straight path
                return true;
            } else {
                //no
                return false;
            }
        }

        protected abstract void OnCantMove<T>(T component)
			where T : Component;
	}

}
