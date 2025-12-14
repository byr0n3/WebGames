const PLAYING_CARD_ATTRIBUTE = 'data-playing-card';
const PLAYING_CARD_DATASET_KEY = 'playingCard';

window.Solitaire = class {
	#helper;
	/** @type {MutationObserver} */
	#observer;

	constructor(dotNetHelper) {
		this.#helper = dotNetHelper;
		this.#observer = new MutationObserver(this.#onObserved.bind(this));

		const container = document.getElementById('solitaire-container');

		this.#observer.observe(container, {
			attributeFilter: [PLAYING_CARD_ATTRIBUTE],
			attributes: true,
			childList: true,
			subtree: true,
		});

		document.querySelectorAll('[data-playing-card]:not([data-playing-card="0"])')
			.forEach(this.#setupElement);
	}

	/**
	 * @param mutations {MutationRecord[]}
	 */
	#onObserved(mutations) {
		for (const mutation of mutations) {
			const element = mutation.target;

			if (!(element instanceof SVGElement) || !element.dataset[PLAYING_CARD_DATASET_KEY]) {
				continue;
			}

			this.#setupElement(element);
		}
	}

	/**
	 * @param element {SVGElement}
	 */
	#setupElement(element) {
		// @todo Setup drag-n-drop
	}
}
