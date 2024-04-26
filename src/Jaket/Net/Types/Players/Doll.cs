namespace Jaket.Net.Types.Players;

using System;
using UnityEngine;

using Jaket.IO;

/// <summary>
/// Doll of a player, remote from network or local from emoji.
/// Responsible for the visual part of the player, i.e. suits and animations.
/// </summary>
public class Doll : MonoBehaviour
{
    /// <summary> Component rendering animations of the doll. </summary>
    public Animator Animator;
    /// <summary> Animator states that affect which animation will be played. </summary>
    public bool Walking, Sliding, Falling, InAir, WasInAir, Dashing, WasDashing, Riding, WasRiding, Hooking, WasHooking, Shopping, WasShopping;

    /// <summary> Emoji that plays at the moment. </summary>
    public byte Emoji, LastEmoji = 0xFF, Rps;
    /// <summary> Event triggered after the start of emoji. </summary>
    public Action OnEmojiStart = () => { };

    /// <summary> Transforms of different parts of the body. </summary>
    public Transform Head, Hand, Hook, HookRoot, Throne, Coin, Skateboard, Suits;
    /// <summary> Slide and fall particles transforms. </summary>
    public Transform SlideParticle, FallParticle;

    /// <summary> Position in which the doll holds an item. </summary>
    public Vector3 HoldPosition => Hooking ? Hook.position : HookRoot.position;

    private void Awake()
    {
        Transform V3 = transform.Find("V3"), rig = transform.Find("Metarig");

        Head = rig.Find("Spine 0/Spine 1/Spine 2");
        Hand = rig.Find("Spine 0/Right Shoulder/Right Elbow/Right Wrist");
        Hand = Tools.Create("Weapons Root", Hand).transform;
        Hook = rig.Find("Hook");
        HookRoot = rig.Find("Spine 0/Left Shoulder/Left Elbow/Left Wrist/Left Palm");
        Throne = rig.Find("Throne");
        Coin = V3.Find("Coin");
        Skateboard = V3.Find("Skateboard");
        Suits = V3.Find("Suits");
    }

    private void Update()
    {
        Animator.SetBool("walking", Walking);
        Animator.SetBool("sliding", Sliding);
        Animator.SetBool("in-air", InAir);
        Animator.SetBool("dashing", Dashing);
        Animator.SetBool("riding", Riding);
        Animator.SetBool("hooking", Hooking);
        Animator.SetBool("shopping", Shopping);

        if (WasInAir != InAir && (WasInAir = InAir)) Animator.SetTrigger("jump");
        if (WasRiding != Riding && (WasRiding = Riding)) Animator.SetTrigger("ride");
        if (WasDashing != Dashing && (WasDashing = Dashing)) Animator.SetTrigger("dash");
        if (WasHooking != Hooking)
        {
            if (WasHooking = Hooking) Animator.SetTrigger("hook");
            Hook.gameObject.SetActive(Hooking);
        }
        if (WasShopping != Shopping && (WasShopping = Shopping)) Animator.SetTrigger("open-shop");

        if (LastEmoji != Emoji)
        {
            Animator.SetTrigger("show-emoji");
            Animator.SetInteger("emoji", LastEmoji = Emoji);
            Animator.SetInteger("rps", Rps);

            Throne.gameObject.SetActive(Emoji == 6);
            Coin.gameObject.SetActive(Emoji == 7);
            Skateboard.gameObject.SetActive(Emoji == 11);

            OnEmojiStart();
        }

        if (Sliding && SlideParticle == null)
        {
            SlideParticle = Instantiate(NewMovement.Instance.slideParticle, transform).transform;
            SlideParticle.localPosition = new(0f, 0f, 3.5f);
            SlideParticle.localEulerAngles = new(0f, 180f, 0f);
            SlideParticle.localScale = new(1.5f, 1f, .8f);
        }
        else if (!Sliding && SlideParticle != null) Destroy(SlideParticle.gameObject);

        if (Falling && FallParticle == null)
        {
            FallParticle = Instantiate(NewMovement.Instance.fallParticle, transform).transform;
            FallParticle.localPosition = new(0f, 6f, 0f);
            FallParticle.localEulerAngles = new(90f, 0f, 0f);
            FallParticle.localScale = new(1.2f, .6f, 1f);
        }
        else if (!Falling && FallParticle != null) Destroy(FallParticle.gameObject);
    }

    #region entity

    public void WriteAnim(Writer w) => w.Bools(Walking, Sliding, Falling, InAir, Dashing, Riding, Hooking, Shopping);

    public void ReadAnim(Reader r) => r.Bools(out Walking, out Sliding, out Falling, out InAir, out Dashing, out Riding, out Hooking, out Shopping);

    #endregion
}
