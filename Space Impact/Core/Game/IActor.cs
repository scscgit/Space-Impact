﻿using Space_Impact.Core.Game;
using Space_Impact.Core.Game.ActorStrategy;
using Space_Impact.Core.Game.PartActor;
using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core
{
	public interface IActor : IAnimatedObject, IPlacedInField, IAct
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

		//Defines possible collision interaction with some other actor
		bool CollidesWith(IActor actor);

		//Strategies of the Actor
		LinkedList<IActStrategy> ActStrategies
		{
			get;
		}
		LinkedList<IDrawModificationStrategy> DrawModificationStrategies
		{
			get;
		}

		LinkedList<IActorCompositePart> ActorComposition
		{
			get;
		}

		bool IntersectsOn(float x, float y);
		bool IntersectsWithin(float x, float width, float y, float height);
		bool IntersectsActor(IActor actor);
	}
}
