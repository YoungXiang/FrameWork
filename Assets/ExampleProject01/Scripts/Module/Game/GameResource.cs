using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResource
{
    // 游戏消耗品或资源
    public enum Consumable
    {
        Population, // 人口: 用于组件民兵
        Wood,       // 木材: 用于建造建筑物
        Iron        // 铁矿: 用于建造兵器
    }

    // 游戏货币
    public enum Currency
    {
        Diamond     // 用来购买木材，铁矿，招募雇佣兵。或用来加速建筑升级等。
    }
}
