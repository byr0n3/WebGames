using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
		/// <summary>
		/// The number of tableau piles used by the <see cref="Solitaire"/> game.
		/// </summary>
		public const int TableauCount = 7;

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

		/// <summary>
		/// The 4 card stacks that need to be filled to win the game.
		/// </summary>
		public readonly List<Card>[] Foundations;

		/// <summary>
		/// The 7 card stacks on the table that are played with.
		/// </summary>
		public readonly List<Card>[] Tableaus;

		/// <summary>
		/// Indicates what card in the tableau stack is visible.
		/// </summary>
		public readonly int[] TableauVisibility;

		/// <summary>
		/// The stack of cards the player can draw.
		/// </summary>
		public readonly List<Card> Talon;

		/// <summary>
		/// Returns the card currently at the top of the talon.
		/// </summary>
		public Card TalonCard =>
			this.talonIndex != -1 ? this.Talon[this.talonIndex] : default;

		/// <summary>
		/// Raised whenever a move is performed or the talon changes, informing subscribers about the updated state of the <see cref="Solitaire"/> instance.
		/// </summary>
		public event OnStateUpdated? StateUpdated;

		/// <summary>
		/// Indicates whether the game can finish because all cards in every tableau stack is visible.
		/// </summary>
		public bool CanFinish =>
			this.TableauVisibility.All(static (visibility) => visibility <= 0);

		/// <summary>
		/// Indicates whether the game has reached its finished state.
		/// </summary>
		public bool IsFinished =>
			this.Foundations.All(static (foundation) => (foundation.Count != 0) && (foundation[^1].Rank is CardRank.King));

		private int talonIndex;

		private Solitaire(string code, GameConfiguration configuration) : base(code, configuration)
		{
			this.Foundations = new List<Card>[Solitaire.foundationCount];
			this.Tableaus = new List<Card>[Solitaire.TableauCount];
			this.TableauVisibility = new int[Solitaire.TableauCount];
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

					this.TableauVisibility[i] = i;
				}

				this.Talon.Clear();
				this.Talon.EnsureCapacity(stack.Remaining);

				stack.Take(this.Talon);
			}

			base.Start();
		}

		/// <summary>
		/// Restarts the game by reinitializing its current state.
		/// </summary>
		public void Restart()
		{
			if (this.State == GameState.Idle)
			{
				return;
			}

			this.Start();
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

			if ((this.talonIndex + 1) >= this.Talon.Count)
			{
				this.talonIndex = -1;
			}
			else
			{
				this.talonIndex++;
			}

			this.StateUpdated?.Invoke(this, new SolitaireStateUpdatedArgs(StackType.Talon, default, StackType.Talon, default));
		}

		/// <summary>
		/// Moves a card or a sequence of cards from one stack to another within the Solitaire game.
		/// </summary>
		/// <param name="srcType">The type of the stack from which the card(s) will be moved.</param>
		/// <param name="srcIndex">The index of the source stack. If <paramref name="srcType"/> is <see cref="StackType.Talon"/>, this value is ignored and the current talon index is used instead.</param>
		/// <param name="dstType">The type of the stack to which the card(s) will be moved.</param>
		/// <param name="dstIndex">The index of the destination stack. If <paramref name="dstType"/> is <see cref="StackType.Foundation"/>, this value is overridden to correspond to the suit of the card being moved.</param>
		public void Move(StackType srcType, int srcIndex, StackType dstType, int dstIndex)
		{
			if (this.State != GameState.Playing)
			{
				return;
			}

			if (srcType == StackType.Talon)
			{
				srcIndex = this.talonIndex;
			}

			this.ValidateMove(srcType, srcIndex, dstType, dstIndex);

			var src = (srcType) switch
			{
				StackType.Tableau    => this.Tableaus[srcIndex],
				StackType.Foundation => this.Foundations[srcIndex],
				_                    => this.Talon,
			};

			var srcCard = (src.Count != 0)
				? (srcType) switch
				{
					StackType.Tableau    => dstType == StackType.Foundation ? src[^1] : src[this.TableauVisibility[srcIndex]],
					StackType.Foundation => src[^1],
					_                    => this.TalonCard,
				}
				: default;

			if (dstType == StackType.Foundation)
			{
				dstIndex = ((int)srcCard.Suit) - 1;
			}

			var dst = dstType switch
			{
				StackType.Tableau    => this.Tableaus[dstIndex],
				StackType.Foundation => this.Foundations[dstIndex],
				_                    => this.Talon,
			};

			var dstCard = (dst.Count != 0)
				? (dstType) switch
				{
					StackType.Tableau    => dst[^1],
					StackType.Foundation => dst[^1],
					_                    => this.TalonCard,
				}
				: default;

			if (!Solitaire.IsMoveValid(dstType, dstCard, srcCard))
			{
				return;
			}

			this.MoveCards(srcType, srcIndex, src, srcCard, dstType, dst);

			// ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
			switch (srcType)
			{
				case StackType.Tableau:
					this.TableauVisibility[srcIndex] = int.Max(this.TableauVisibility[srcIndex] - 1, 0);
					break;

				case StackType.Talon:
					this.talonIndex--;
					break;
			}

			if (this.IsFinished)
			{
				this.Stop();
			}
			else
			{
				this.StateUpdated?.Invoke(this, new SolitaireStateUpdatedArgs(srcType, srcIndex, dstType, dstIndex));
			}
		}

		/// <summary>
		/// Transfers card(s) between two stacks within the game.
		/// </summary>
		/// <param name="srcType">The <see cref="StackType"/> of the stack from which cards will be moved.</param>
		/// <param name="srcIndex">The index of the source stack within its collection.</param>
		/// <param name="src">The list that holds the cards of the source stack.</param>
		/// <param name="srcCard">The card that is the primary element to be moved.</param>
		/// <param name="dstType">The <see cref="StackType"/> of the stack to which cards will be added.</param>
		/// <param name="dst">The list that holds the cards of the destination stack.</param>
		private void MoveCards(StackType srcType, int srcIndex, List<Card> src, Card srcCard, StackType dstType, List<Card> dst)
		{
			if ((srcType is StackType.Tableau) && (dstType is StackType.Tableau))
			{
				var topCount = src.Count;
				var visible = this.TableauVisibility[srcIndex];

				for (var i = visible; i < topCount; i++)
				{
					dst.Add(src[i]);
				}

				src.RemoveRange(visible, src.Count - visible);
			}
			else
			{
				dst.Add(srcCard);
				src.Remove(srcCard);
			}
		}

		[SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault")]
		private void ValidateMove(StackType srcType, int srcIndex, StackType dstType, int dstIndex)
		{
			switch (srcType)
			{
				case StackType.Tableau when (srcIndex < 0) || (srcIndex > this.Tableaus.Length):
					throw new ArgumentException($"Invalid tableau index: {srcIndex}", nameof(srcIndex));

				case StackType.Foundation when (srcIndex < 0) || (srcIndex > this.Foundations.Length):
					throw new ArgumentException($"Invalid foundation index: {srcIndex}", nameof(srcIndex));
			}

			switch (dstType)
			{
				case StackType.Tableau when (dstIndex < 0) || (dstIndex > this.Tableaus.Length):
					throw new ArgumentException($"Invalid tableau index: {dstIndex}", nameof(dstIndex));

				case StackType.Foundation when (dstIndex < 0) || (dstIndex > this.Foundations.Length):
					throw new ArgumentException($"Invalid foundation index: {dstIndex}", nameof(dstIndex));
			}
		}

		private static bool IsMoveValid(StackType dstType, Card dst, Card src)
		{
			// If there's no bottom card, the top card HAS to be a king.
			if (dst == default)
			{
				return src.Rank == (dstType == StackType.Foundation ? CardRank.Ace : CardRank.King);
			}

			if (!Solitaire.IsMoveValid(dst.Rank, src.Rank))
			{
				return false;
			}

			return (dstType == StackType.Foundation) || Solitaire.IsMoveValid(dst.Suit, src.Suit);
		}

		private static bool IsMoveValid(CardRank dst, CardRank src) =>
			int.Abs((int)src - (int)dst) == 1;

		// @todo Refactor
		private static bool IsMoveValid(CardSuit dst, CardSuit src) =>
			(src) switch
			{
				CardSuit.Heart   => dst is CardSuit.Spade or CardSuit.Club,
				CardSuit.Diamond => dst is CardSuit.Spade or CardSuit.Club,
				CardSuit.Spade   => dst is CardSuit.Heart or CardSuit.Diamond,
				CardSuit.Club    => dst is CardSuit.Heart or CardSuit.Diamond,
				_                => throw new ArgumentException("Invalid suit", nameof(src)),
			};

		/// <inheritdoc/>
		public static IGame Create(string code, GameConfiguration configuration, IServiceProvider _) =>
			new Solitaire(code, configuration);

		/// <summary>
		/// Specifies the various types of card stacks that can exist in a solitaire game.
		/// These values are used when moving cards or querying the state of the game.
		/// </summary>
		public enum StackType
		{
			/// <summary>
			/// Invalid default value.
			/// </summary>
			Invalid,

			/// <summary>
			/// A stack that represents one of the tableau piles in a solitaire game.
			/// </summary>
			Tableau,

			/// <summary>
			/// A stack that represents one of the foundation piles in a solitaire game.
			/// </summary>
			Foundation,

			/// <summary>
			/// A stack that represents the talon pile in a solitaire game.
			/// </summary>
			Talon,
		}
	}
}
