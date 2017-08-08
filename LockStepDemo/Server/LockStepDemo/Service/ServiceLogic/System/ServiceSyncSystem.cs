﻿using DeJson;
using LockStepDemo.GameLogic.Component;
using LockStepDemo.GameLogic.System;
using LockStepDemo.Service;
using LockStepDemo.Service.ServiceLogic.Component;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LockStepDemo.ServiceLogic.System
{
    class ServiceSyncSystem : ServiceSystem
    {
        public override void Init()
        {   
            
        }

        public override Type[] GetFilter()
        {
            return new Type[] {
                typeof(WaitSyncComponent)
            };
        }

        public override void LateFixedUpdate(int deltaTime)
        {
            List<EntityBase> list = GetEntityList();

            for (int i = 0; i < list.Count; i++)
            {
                list[i].RemoveComp<WaitSyncComponent>();

                List<EntityBase> players = GetEntityList(new string[] { "ConnectionComponent" });
                for (int j = 0; j < players.Count; j++)
                {
                    PushSyncEnity(players[i].GetComp<ConnectionComponent>().m_session, list[i]);
                }
            }
        }

        #region 推送数据

        public void PushSyncEnity(SyncSession session, EntityBase entity)
        {
            SyncEntityMsg msg = new SyncEntityMsg();
            msg.m_id = entity.ID;
            msg.infos = new List<ComponentInfo>();

            foreach (var c in entity.m_compDict)
            {
                Type type = c.Value.GetType();

                if(!type.IsSubclassOf(typeof(ServiceComponent)))
                {
                    ComponentInfo info = new ComponentInfo();
                    info.m_compName = type.Name;
                    info.content = Serializer.Serialize(c.Value);
                    msg.infos.Add(info);
                }
            }
            session.SendMsg(msg);
        }

        #endregion

        #region 接收数据

        public void ReceviceClient(SyncSession session, ChangeComponentMsg msg)
        {

        }

        #endregion
    }
}