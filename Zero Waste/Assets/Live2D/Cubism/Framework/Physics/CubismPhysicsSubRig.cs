﻿/*
 * Copyright(c) Live2D Inc. All rights reserved.
 * 
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at http://live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;
using Live2D.Cubism.Core;
using UnityEngine;


namespace Live2D.Cubism.Framework.Physics
{
    /// <summary>
    /// Children of rig.
    /// </summary>
    [Serializable]
    public class CubismPhysicsSubRig
    {
        /// <summary>
        /// Input.
        /// </summary>
        [SerializeField]
        public CubismPhysicsInput[] Input;

        /// <summary>
        /// Output.
        /// </summary>
        [SerializeField]
        public CubismPhysicsOutput[] Output;
        
        /// <summary>
        /// Particles.
        /// </summary>
        [SerializeField]
        public CubismPhysicsParticle[] Particles;

        /// <summary>
        /// Normalization.
        /// </summary>
        [SerializeField]
        public CubismPhysicsNormalization Normalization;
        
        /// <summary>
        /// Rig.
        /// </summary>
        public CubismPhysicsRig Rig
        {
            get { return _rig; }
            set { _rig = value; }
        }

        [NonSerialized]
        private CubismPhysicsRig _rig;
        
        
        /// <summary>
        /// Updates parameter from output value.
        /// </summary>
        /// <param name="parameter">Target parameter.</param>
        /// <param name="translation">Translation.</param>
        /// <param name="output">Output value.</param>
        private void UpdateOutputParameterValue(CubismParameter parameter, float translation, CubismPhysicsOutput output)
        {
            var outputScale = 1.0f;
            
            outputScale = output.GetScale();

            var value = translation * outputScale;


            if (value < parameter.MinimumValue)
            {
                if (value < output.ValueBelowMinimum)
                {
                    output.ValueBelowMinimum = value;
                }


                value = parameter.MinimumValue;
            }
            else if (value > parameter.MaximumValue)
            {
                if (value > output.ValueExceededMaximum)
                {
                    output.ValueExceededMaximum = value;
                }


                value = parameter.MaximumValue;
            }


            var weight = (output.Weight / CubismPhysics.MaximumWeight);

            if (weight >= 1.0f)
            {
                parameter.Value = value;
            }
            else
            {
                value = (parameter.Value * (1.0f - weight)) + (value * weight);
                parameter.Value = value;
            }
        }


        /// <summary>
        /// Updates particles in every frame.
        /// </summary>
        /// <param name="strand">Particles.</param>
        /// <param name="totalTranslation">Total translation.</param>
        /// <param name="totalAngle">Total angle.</param>
        /// <param name="wind">Direction of wind.</param>
        /// <param name="thresholdValue">Value of threshold.</param>
        /// <param name="deltaTime">Time of delta.</param>
        private void UpdateParticles(
            CubismPhysicsParticle[] strand,
            Vector2 totalTranslation,
            float totalAngle,
            Vector2 wind,
            float thresholdValue,
            float deltaTime
            )
        {
            strand[0].Position = totalTranslation;
            
            var totalRadian = CubismPhysicsMath.DegreesToRadian(totalAngle);
            var currentGravity = CubismPhysicsMath.RadianToDirection(totalRadian);
            currentGravity.Normalize();

            for (var i = 1; i < strand.Length; ++i)
            {
                strand[i].Force = (currentGravity * strand[i].Acceleration) + wind;

                strand[i].LastPosition = strand[i].Position;

                // The Cubism Editor expects 30 FPS so we scale here by 30...
                var delay = strand[i].Delay * deltaTime * 30.0f;

                var direction = strand[i].Position - strand[i - 1].Position;
                var radian = CubismPhysicsMath.DirectionToRadian(strand[i].LastGravity, currentGravity) / CubismPhysics.AirResistance;

                
                direction.x = ((Mathf.Cos(radian) * direction.x) - (direction.y * Mathf.Sin(radian)));
                direction.y = ((Mathf.Sin(radian) * direction.x) + (direction.y * Mathf.Cos(radian)));


                strand[i].Position = strand[i - 1].Position + direction;


                var velocity = strand[i].Velocity * delay;
                var force = strand[i].Force * delay * delay;


                strand[i].Position = strand[i].Position + velocity + force;


                var newDirection = strand[i].Position - strand[i - 1].Position;

                newDirection.Normalize();


                strand[i].Position = strand[i - 1].Position + newDirection * strand[i].Radius;

                if (Mathf.Abs(strand[i].Position.x) < thresholdValue)
                {
                    strand[i].Position.x = 0.0f;
                }


                if (delay != 0.0f)
                {
                    strand[i].Velocity =
                            ((strand[i].Position - strand[i].LastPosition) / delay) * strand[i].Mobility;
                }


                strand[i].Force = Vector2.zero;
                strand[i].LastGravity = currentGravity;
            }
        }


        /// <summary>
        /// Initializes <see langword="this"/>.
        /// </summary>
        public void Initialize()
        {
            var strand = Particles;

            // Initialize the top of particle.
            strand[0].InitialPosition = Vector2.zero;
            strand[0].LastPosition = strand[0].InitialPosition;
            strand[0].LastGravity = CubismPhysics.Gravity;
            strand[0].LastGravity.y *= -1.0f;


            // Initialize paritcles.
            for (var i = 1; i < strand.Length; ++i)
            {
                var radius = Vector2.zero;
                radius.y = strand[i].Radius;
                strand[i].InitialPosition = strand[i - 1].InitialPosition + radius;
                strand[i].Position = strand[i].InitialPosition;
                strand[i].LastPosition = strand[i].InitialPosition;
                strand[i].LastGravity = CubismPhysics.Gravity;
                strand[i].LastGravity.y *= -1.0f;
            }

            
            // Initialize inputs.
            for (var i = 0; i < Input.Length; ++i)
            {
                Input[i].InitializeGetter();
            }

            // Initialize outputs.
            for (var i = 0; i < Output.Length; ++i)
            {
                Output[i].InitializeGetter();
            }
        }
        

        /// <summary>
        /// Evalute rig in every frame.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Evaluate(float deltaTime)
        {
            var totalAngle = 0.0f;
            var totalTranslation = Vector2.zero;

            
            for (var i = 0; i < Input.Length; ++i)
            {
                var weight = Input[i].Weight / CubismPhysics.MaximumWeight;


                if (Input[i].Source == null)
                {
                    Input[i].Source = Rig.Controller.Parameters.FindById(Input[i].SourceId);
                }


                var parameter = Input[i].Source;
                Input[i].GetNormalizedParameterValue(
                    ref totalTranslation,
                    ref totalAngle,
                    parameter,
                    Normalization,
                    weight
                    );
                
            }
            

            var radAngle = CubismPhysicsMath.DegreesToRadian(-totalAngle);
            

            totalTranslation.x = (totalTranslation.x * Mathf.Cos(radAngle) - totalTranslation.y * Mathf.Sin(radAngle));
            totalTranslation.y = (totalTranslation.x * Mathf.Sin(radAngle) + totalTranslation.y * Mathf.Cos(radAngle));
            

            UpdateParticles(
                Particles,
                totalTranslation,
                totalAngle,
                CubismPhysics.Wind,
                CubismPhysics.MovementThreshold * Normalization.Position.Maximum,
                deltaTime
                );
                        
            
            for (var i = 0; i < Output.Length; ++i)
            {
                var particleIndex = Output[i].ParticleIndex;

                if (particleIndex < 1 || particleIndex >= Particles.Length)
                {
                    break;
                }

                if (Output[i].Destination == null)
                {
                    Output[i].Destination = Rig.Controller.Parameters.FindById(Output[i].DestinationId);
                }

                var parameter = Output[i].Destination;
                
                var translation = Particles[particleIndex].Position -
                                        Particles[particleIndex - 1].Position;
                
                var outputValue = Output[i].GetValue(
                    translation, 
                    parameter, 
                    Particles, 
                    particleIndex
                    );
                

                UpdateOutputParameterValue(parameter, outputValue, Output[i]);
            }
        }
    }
}