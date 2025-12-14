using System;
using System.Collections.Generic;
using System.Diagnostics;
using WebGames.Core.Cards;
using WebGames.Core.Events;
using WebGames.Core.Players;

namespace WebGames.Core.Games
{
	/// <summary>
	/// Represents a single‑player solitaire game that can be instantiated.
	/// </summary>
	public sealed class Solitaire : BaseGame<SolitairePlayer>, ICreatableGame
	{
		private const int tableauCount = 7;
		private static readonly int foundationCount = Enum.GetValues<CardSuit>().Length - 1;

		/// <summary>
		/// Invoked when the state of a <see cref="Solitaire"/> game changes.
		/// </summary>
		/// <param name="game">The game instance whose state has been updated.</param>
		/// <param name="args">Data describing the state transition.</param>
		public delegate void OnStateUpdated(Solitaire game, SolitaireStateUpdatedArgs args);

		/// <summary>
		/// Defines the default configuration used when creating a new <see cref="Solitaire"/> instance.
		/// </summary>
		public static readonly GameConfiguration DefaultConfiguration = new()
		{
			MinPlayers = 1,
			MaxPlayers = 1,
			AutoStart = true,
		};

		// The 4 card stacks that need to be filled to win the game.
		public readonly List<Card>[] Foundations;

		// The 7 card stacks on the table that are played with.
		public readonly List<Card>[] Tableaus;

		// The stack of cards the player can draw.
		public readonly List<Card> Talon;

		private int talonIndex;

		public Card TalonCard =>
			this.talonIndex != -1 ? this.Talon[this.talonIndex] : default;

		public event OnStateUpdated? StateUpdated;

		private Solitaire(string code, GameConfiguration configuration) : base(code, configuration)
		{
			this.Foundations = new List<Card>[Solitaire.foundationCount];
			this.Tableaus = new List<Card>[Solitaire.tableauCount];
			this.Talon = [];

			for (var i = 0; i < this.Foundations.Length; i++)
			{
				this.Foundations[i] = [];
			}

			for (var i = 0; i < this.Tableaus.Length; i++)
			{
				this.Tableaus[i] = [];
			}
		}

		/// <inheritdoc/>
		protected override void Start()
		{
			this.talonIndex = -1;

			foreach (var foundation in this.Foundations)
			{
				foundation.Clear();
			}

			using (var stack = new CardStack(1))
			{
				for (var i = 0; i < this.Tableaus.Length; i++)
				{
					var tableau = this.Tableaus[i];

					tableau.Clear();

					stack.Take(tableau, i + 1);
				}

				this.Talon.Clear();
				this.Talon.EnsureCapacity(stack.Remaining);

				stack.Take(this.Talon);
			}

			base.Start();
		}

		/// <summary>
		/// Advances to the next card in the talon sequence.
		/// </summary>
		public void NextTalonCard()
		{
			if (this.State != GameState.Playing)
			{
				return;
			}

			this.talonIndex = (this.talonIndex + 1) % this.Talon.Count;

			this.StateUpdated?.Invoke(this, SolitaireStateUpdatedArgs.TalonCardUpdated(this.talonIndex));
		}

		/// <summary>
		/// Moves the current talon card to its corresponding foundation stack, if the move is valid.
		/// </summary>
		public void MoveTalonCardToFoundation()
		{
			if (this.State != GameState.Playing)
			{
				return;
			}

			var card = this.TalonCard;
			var foundationIndex = ((int)card.Suit) - 1;

			Debug.Assert((foundationIndex >= 0) && (foundationIndex <= this.Foundations.Length - 1));

			var foundation = this.Foundations[foundationIndex];
			var diff = (int)card.Rank - (foundation.Count != 0 ? (int)foundation[^1].Rank : 0);

			if (diff != 1)
			{
				return;
			}

			foundation.Add(card);
			this.talonIndex--;

			this.StateUpdated?.Invoke(this, SolitaireStateUpdatedArgs.TalonCardToFoundation());
		}

		/// <summary>
		/// Moves the top card from the specified tableau to its corresponding foundation stack, if the move is valid.
		/// </summary>
		/// <param name="tableauIndex">
		/// The zero‑based index of the tableau from which to attempt to move the top card.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when <paramref name="tableauIndex"/> is less than 0 or greater than or equal
		/// to the number of tableau piles.
		/// </exception>
		public void MoveTableauCardToFoundation(int tableauIndex)
		{
			if (this.State != GameState.Playing)
			{
				return;
			}

			if ((tableauIndex < 0) || (tableauIndex > this.Tableaus.Length))
			{
				throw new ArgumentException($"Invalid tableau index: {tableauIndex}", nameof(tableauIndex));
			}

			var tableau = this.Tableaus[tableauIndex];
			var card = tableau[^1];

			var foundationIndex = ((int)card.Suit) - 1;

			Debug.Assert((foundationIndex >= 0) && (foundationIndex <= this.Foundations.Length - 1));

			var foundation = this.Foundations[foundationIndex];
			var diff = (int)card.Rank - (foundation.Count != 0 ? (int)foundation[^1].Rank : 0);

			if (diff != 1)
			{
				return;
			}

			foundation.Add(card);
			tableau.RemoveAt(tableau.Count - 1);

			this.StateUpdated?.Invoke(this, SolitaireStateUpdatedArgs.TableauCardToFoundation(tableau.Count));
		}

		/// <inheritdoc/>
		public static IGame Create(string code, GameConfiguration configuration, IServiceProvider _) =>
			new Solitaire(code, configuration);
	}
}
