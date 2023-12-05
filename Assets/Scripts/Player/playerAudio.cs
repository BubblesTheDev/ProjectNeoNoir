using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAudio : MonoBehaviour
{
    private playerMovement movementReference;
    private playerHealth healthReference;


    private void Awake()
    {
        movementReference = GetComponent<playerMovement>();
        healthReference = GetComponent<playerHealth>();

        if (movementReference != null && healthReference != null) addListeners();
    }

    void addListeners()
    {
        movementReference.onAction_Jump_Start.AddListener(playJumpSound);
        movementReference.onAction_SlideJumpStart.AddListener(playSlideJumpSound);
        movementReference.onAction_Dash_Start.AddListener(playDashSound);
        movementReference.onAction_Slide_Start.AddListener(startSlideSound);
        movementReference.onAction_Slam_Start.AddListener(startSlamSound);
        movementReference.onAction_Flip_Start.AddListener(playNormalFlipSound);
        movementReference.onAction_Slide_End.AddListener(stopSlideSound);
        movementReference.onAction_Slam_End.AddListener(playSlamEndSound);
        movementReference.onAction_Flip_End.AddListener(playFlippedFlipSound);
        movementReference.onAction_CannotAirJump.AddListener(playCannotAirJumpSound);
        movementReference.onAction_CannotDash.AddListener(playCannotDashSound);
        movementReference.onAction_CannotFlip.AddListener(playCannotFlipSound);
        movementReference.onAction_OverchargeFlip.AddListener(playOverchargeFlipSound);
        healthReference.tookDamage.AddListener(playDanageSound);
        healthReference.healedDamage.AddListener(playHealedSound);
    }

    private void playJumpSound()
    {
        AudioManager.instance.PlaySFX(FMODEvents.instance.jumpSFX, this.transform.position);
    }
    private void playSlideJumpSound()
    {

    }
    private void playDashSound()
    {
        AudioManager.instance.PlaySFX(FMODEvents.instance.dashSFX, this.transform.position);
    }
    private void startSlideSound()
    {

    }
    private void startSlamSound()
    {

    }
    private void playNormalFlipSound()
    {
        AudioManager.instance.PlaySFX(FMODEvents.instance.gravitySwitch, this.transform.position);
    }
    private void stopSlideSound()
    {

    }
    private void playSlamEndSound()
    {

    }
    private void playFlippedFlipSound()
    {
        AudioManager.instance.PlaySFX(FMODEvents.instance.gravitySwitch, this.transform.position);
    }
    private void playCannotAirJumpSound()
    {

    }
    private void playCannotDashSound()
    {

    }
    private void playCannotFlipSound()
    {

    }
    private void playOverchargeFlipSound()
    {

    }

    private void playDanageSound()
    {

    }

    private void playHealedSound()
    {

    }

}
