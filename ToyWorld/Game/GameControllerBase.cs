﻿using System.Collections.Generic;
using GoodAI.ToyWorld.Control;
using Render.Renderer;
using Render.RenderRequests;
using TmxMapSerializer.Elements;
using TmxMapSerializer.Serializer;
using World.GameActors.GameObjects;
using World.ToyWorldCore;
using World.WorldInterfaces;

namespace Game
{
    // TODO: why there is abstract class instead of BasicGameController?
    public abstract class GameControllerBase : IGameController
    {
        public RendererBase Renderer { get; private set; }
        public IWorld World { get; private set; }
        private readonly GameSetup m_gameSetup;
        private Dictionary<int, Avatar> m_avatars;
        private Dictionary<int, AvatarController> m_avatarControllers;


        protected GameControllerBase(RendererBase renderer, GameSetup setup)
        {
            Renderer = renderer;
            m_gameSetup = setup;
        }

        public virtual void Dispose()
        {
            if (Renderer != null)
                Renderer.Dispose();
            Renderer = null;
        }


        #region IGameController overrides

        public virtual void Init()
        {
            // Init World
            var serializer = new TmxSerializer();
            Map map = serializer.Deserialize(m_gameSetup.SaveFile);

            World = new ToyWorld(map, m_gameSetup.TilesetFile);

            m_avatars = new Dictionary<int, Avatar>();
            foreach (int avatarId in World.GetAvatarsIds())
            {
                m_avatars.Add(avatarId, World.GetAvatar(avatarId));
            }

            m_avatarControllers = new Dictionary<int, AvatarController>();
            foreach (KeyValuePair<int, Avatar> avatar in m_avatars)
            {
                m_avatarControllers.Add(avatar.Key, new AvatarController(avatar.Value));
            }

            // Init rendering
            Renderer.Init();
            Renderer.CreateWindow("TestGameWindow", 1024, 1024);
            Renderer.CreateContext();
        }

        public virtual void MakeStep()
        {
            // Assume Init has been called, we don't want to check for consistency every step

            // World
            World.Update();

            foreach (AvatarController avatarController in m_avatarControllers.Values)
            {
                avatarController.ResetControls();
            }

            // Rendering
            Renderer.ProcessRequests();
        }


        public virtual T RegisterRenderRequest<T>()
            where T : class, IRenderRequest
        {
            Renderer.MakeContextCurrent();
            var rr = RenderRequestFactory.CreateRenderRequest<T>();
            InitRR(rr);
            Renderer.EnqueueRequest(rr);
            Renderer.MakeContextNotCurrent();

            return rr;
        }

        public virtual T RegisterRenderRequest<T>(int avatarId)
            where T : class, IAvatarRenderRequest
        {
            // TODO: check agentID or make the param an AgentController?

            Renderer.MakeContextCurrent();
            T rr = RenderRequestFactory.CreateRenderRequest<T>(avatarId);
            InitRR(rr);
            Renderer.EnqueueRequest(rr);
            Renderer.MakeContextNotCurrent();

            return rr;
        }

        void InitRR<T>(T rr)
            where T : class
        {
            RenderRequest rrBase = rr as RenderRequest; // Assume that all renderRequests created by factory inherit from RenderRequest

            if (rrBase == null)
                throw new RenderRequestNotImplementedException(string.Format("Incorrect type argument; the type {0} is not registered for use in this controller version.", typeof(T).Name));

            rrBase.Init(Renderer);
        }


        public IAvatarController GetAvatarController(int avatarId)
        {
            return m_avatarControllers[avatarId];
        }

        #endregion
    }
}
