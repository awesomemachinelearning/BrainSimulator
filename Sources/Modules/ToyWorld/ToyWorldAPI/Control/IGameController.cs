﻿using System;
using System.Collections.Generic;
using GoodAI.ToyWorldAPI;

namespace GoodAI.ToyWorld.Control
{
    /// <summary>
    ///
    /// </summary>
    public interface IGameController : IDisposable, IMessanger
    {
        /// <summary>
        /// Path of a lua script that gets executed just once when MakeStep is called
        /// </summary>
        string LuaScriptPath { get; set; }

        /// <summary>
        /// Enables thorough synchronization - ToyWorldCore.ToyWorld waits in Update() for a signal from script
        /// </summary>
        bool LuaThoroughSync { get; set; }

        /// <summary>
        ///
        /// </summary>
        void Init();

        /// <summary>
        ///
        /// </summary>
        void MakeStep();

        void FinishStep();

        /// <summary>
        ///
        /// </summary>
        /// <param name="avatarID"></param>
        /// <exception cref="RenderRequestNotImplementedException">Thrown when requesting an unknown <see cref="IAvatarRenderRequest"/> from the controller.
        /// This usually indicates an older version of the core than the API.</exception>
        T RegisterRenderRequest<T>(int avatarID)
            where T : class, IAvatarRenderRequest;

        /// <summary>
        ///
        /// </summary>
        /// <exception cref="RenderRequestNotImplementedException">Thrown when requesting an unknown <see cref="IRenderRequest"/> from the controller.
        /// This usually indicates an older version of the core than the API.</exception>
        T RegisterRenderRequest<T>()
            where T : class, IRenderRequest;

        /// <summary>
        ///
        /// </summary>
        /// <param name="avatarId"></param>
        /// <returns></returns>
        IAvatarController GetAvatarController(int avatarId);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        int[] GetAvatarIds();

        /// <summary>
        /// Returns results from signal dispatchers of IWorld
        /// </summary>
        /// <returns></returns>
        Dictionary<string, float> GetSignals();
    }
}
