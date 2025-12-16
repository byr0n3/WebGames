// @todo Localize
const MSG = 'Are you sure you want to do this?';

function hook(event) {
	event.preventDefault();

	event.returnValue = true;

	return MSG;
}

function unhook() {
	window.onbeforeunload = undefined;
}

window.onbeforeunload = hook;
window.onunload = unhook;

Blazor.addEventListener('enhancednavigationstart', unhook);

document.querySelectorAll('a')
	.forEach(function (clickable) {
		clickable.addEventListener('click', function (event) {
			if (!confirm(MSG)) {
				event.preventDefault();
				event.stopImmediatePropagation();
			}
		});
	});
