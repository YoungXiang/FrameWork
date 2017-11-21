using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artemis;
using Artemis.Interface;
using FrameWork;

[Artemis.Attributes.ArtemisEntityTemplate("Character")]
public class CharacterTemplate : IEntityTemplate
{
    public string prefabPath;

    public Entity BuildEntity(Entity e, EntityWorld entityWorld, params object[] args)
    {
        CharacterConfig config = EntityWorldRegistry.Instance.charConfigs[(int)args[0]];

        View view = e.AddComponentFromPool<View>();
        view.Load(config.prefab);
        view.gameObject.name = config.name;

        e.AddComponentFromPool<Velocity>().value = Vector3.one * config.moveSpeed;

        e.AddComponentFromPool<MovePath>();

        return e;
    }
}
