﻿using Space_Impact.Core.Game;
using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core
{
	public interface IActor : IAnimatedObject
	{
		IField Field
		{
			get;
		}

		string Name
		{
			set; get;
		}

		SpaceDirection Direction
		{
			get; set;
		}

		float Speed
		{
			get; set;
		}

		LinkedList<IActorCompositePart> ActorComposition
		{
			get;
		}

		bool CollidesOn(float x, float y);
		bool IntersectsActor(IActor actor);

		void Act();
		void AddedToField(IField field);
		void AddedToFieldHook();

		void DeleteActor();
	}
}
