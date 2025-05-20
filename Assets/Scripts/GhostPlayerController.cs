using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class GhostPlayerController : Playercontroller
{
    private List<InputSnapshot> replayInputs;
    private int currentIndex = 0;
    public float replayInterval = 0.1f;

    public void InitReplay(List<InputSnapshot> inputData)
    {
        replayInputs = new List<InputSnapshot>(inputData);
        StartCoroutine(ReplayInputs());
    }

    private IEnumerator ReplayInputs()
    {
        while (currentIndex < replayInputs.Count)
        {
            var input = replayInputs[currentIndex];

            // Feed inputs directly
            moveInput = input.move;
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);

            runHeld = input.run;
            IsRunning = runHeld;

            if (input.jump)
            {
                // Simulate jump logic
                if (IsAlive && !isHanging && touchingDirections.IsGrounded && CanMove)
                {
                    animator.SetTrigger(AnimationStrings.jumpTrigger);
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
                }
            }

            if (input.attack)
            {
                animator.SetTrigger(AnimationStrings.attackTrigger);
            }

            currentIndex++;
            yield return new WaitForSeconds(replayInterval);
        }

        // Wait a bit before disappearing
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
