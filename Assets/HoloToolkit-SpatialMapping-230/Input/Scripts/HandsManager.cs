// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine.VR.WSA.Input;
using UnityEngine;

namespace Academy.HoloToolkit.Unity
{
    /// <summary>
    /// HandsDetected determines if the hand is currently detected or not.
    /// </summary>
    public partial class HandsManager : Singleton<HandsManager>
    {
        [Tooltip("Audio clip to play when Finger Pressed.")]
        public AudioClip FingerPressedSound;
        private AudioSource audioSource;

        /// <summary>
        /// HandDetected tracks the hand detected state.
        /// Returns true if the list of tracked hands is not empty.
        /// </summary>
        public bool HandDetected
        {
            get { return trackedHands.Count > 0; }
            // private set;
            private set { }
        }

        // Keeps track of the GameObject that the hand is interacting with.
        public GameObject FocusedGameObject { get; private set; }

        private List<uint> trackedHands = new List<uint>();

        void Awake()
        {
            EnableAudioHapticFeedback();

            InteractionManager.SourceDetected += InteractionManager_SourceDetected;
            InteractionManager.SourceLost += InteractionManager_SourceLost;

            /* TODO: DEVELOPER CODE ALONG 2.a */

            // 2.a: Register for SourceManager.SourcePressed event.
            InteractionManager.SourcePressed += InteractionManager_SourcePressed;

            // 2.a: Register for SourceManager.SourceReleased event.
            InteractionManager.SourceReleased += InteractionManager_SourceReleased;

            // 2.a: Initialize FocusedGameObject as null.
            FocusedGameObject = null;
        }

        private void EnableAudioHapticFeedback()
        {
            // If this hologram has an audio clip, add an AudioSource with this clip.
            if (FingerPressedSound != null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }

                audioSource.clip = FingerPressedSound;
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1;
                audioSource.dopplerLevel = 0;
            }
        }

        private void InteractionManager_SourceDetected(InteractionSourceState state)
        {
            // Check to see that the source is a hand.
            if (state.source.kind != InteractionSourceKind.Hand)
            {
                return;
            }

            trackedHands.Add(state.source.id);

            HandDetected = true;
        }

        private void InteractionManager_SourceLost(InteractionSourceState state)
        {
            // Check to see that the source is a hand.
            if (state.source.kind != InteractionSourceKind.Hand)
            {
                return;
            }

            if (trackedHands.Contains(state.source.id))
            {
                trackedHands.Remove(state.source.id);
            }

            HandDetected = false;

            // 2.a: Reset FocusedGameObject.
            ResetFocusedGameObject();
        }

        private void InteractionManager_SourcePressed(InteractionSourceState hand)
        {
            if (InteractibleManager.Instance.FocusedGameObject != null)
            {
                // Play a select sound if we have an audio source and are not targeting an asset with a select sound.
                if (audioSource != null && !audioSource.isPlaying &&
                    (InteractibleManager.Instance.FocusedGameObject.GetComponent<Interactible>() != null &&
                    InteractibleManager.Instance.FocusedGameObject.GetComponent<Interactible>().TargetFeedbackSound == null))
                {
                    audioSource.Play();
                }

                // 2.a: Cache InteractibleManager's FocusedGameObject in FocusedGameObject.
                FocusedGameObject = InteractibleManager.Instance.FocusedGameObject;
            }
        }

        private void InteractionManager_SourceReleased(InteractionSourceState hand)
        {
            // 2.a: Reset FocusedGameObject.
            ResetFocusedGameObject();
        }

        private void ResetFocusedGameObject()
        {
            // 2.a: Set FocusedGameObject to be null.
            FocusedGameObject = null;

            // 2.a: On GestureManager call ResetGestureRecognizers
            // to complete any currently active gestures.
            GestureManager.Instance.ResetGestureRecognizers();
        }

        void OnDestroy()
        {
            InteractionManager.SourceDetected -= InteractionManager_SourceDetected;
            InteractionManager.SourceLost -= InteractionManager_SourceLost;

            // 2.a: Unregister the SourceManager.SourceReleased event.
            InteractionManager.SourceReleased -= InteractionManager_SourceReleased;

            // 2.a: Unregister for SourceManager.SourcePressed event.
            InteractionManager.SourcePressed -= InteractionManager_SourcePressed;
        }
    }
}