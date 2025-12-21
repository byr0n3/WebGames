const COLLAPSABLE = 'collapsable';
const VISIBLE = 'visible';
const DATA_COLLAPSABLE = `data-${COLLAPSABLE}`;
const DATA_CHEVRON = `data-chevron`;

window.Collapsable = {
	registered: [],

	/**
	 * @param trigger {HTMLButtonElement}
	 */
	initialize(trigger) {
		if (window.Collapsable.registered.includes(trigger)) {
			return;
		}

		window.Collapsable.registered.push(trigger);

		const id = trigger.dataset[COLLAPSABLE];
		const visible = Boolean(trigger.dataset[VISIBLE]);

		const content = document.getElementById(id);
		const chevron = trigger.querySelector(`[${DATA_CHEVRON}]`);

		const contentHeight = Math.ceil(content.getBoundingClientRect().height);

		content.style.setProperty('--content-height', `${contentHeight}px`);

		update(visible);

		trigger.addEventListener('click', function () {
			update(!content.classList.contains('visible'));
		});

		function update(visible) {
			if (visible) {
				content.classList.add('visible');
			} else {
				content.classList.remove('visible');
			}

			chevron.style.rotate = visible ? '180deg' : '0deg';
		}
	},

	/**
	 * @param mutations {MutationRecord[]}
	 */
	onMutation(mutations) {
		for (const mutation of mutations) {
			const element = mutation.target;

			if ((element instanceof HTMLButtonElement) && element.dataset.hasOwnProperty(COLLAPSABLE)) {
				window.Collapsable.initialize(element);
			}
		}
	},

	/** @type {MutationObserver} */
	observer: undefined,
};

window.Collapsable.observer = new MutationObserver(window.Collapsable.onMutation);
window.Collapsable.observer.observe(document.body, {
	attributeFilter: [DATA_COLLAPSABLE],
	attributes: true,
	childList: true,
	subtree: true,
});
document.querySelectorAll(`button[${DATA_COLLAPSABLE}]`).forEach(window.Collapsable.initialize);
