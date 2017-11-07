// Auto-gen Version : V 1_0
using System;
using System.Collections.Generic;
using MessagePack;

namespace FrameWork
{
	[MessagePackObject]
	public class Character
	{
		[Key(0)]
		public int id;
		[Key(1)]
		public string name;
		[Key(2)]
		public string gender;
		[Key(3)]
		public int price;
		[Key(4)]
		public string defaultClothes;
		[Key(5)]
		public string assetPath;
		[Key(6)]
		public string iconPath;
	}
	[MessagePackObject]
	public class CharacterData
	{
		[Key(0)]
		public Character[] datas;
	}
}
