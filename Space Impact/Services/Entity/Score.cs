using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Services.Entity
{
	public class Score
	{
		[Key]
		public int Id { get; set; }
		public Player Player { get; set; }
		public int ScoreValue { get; set; }
	}
}
