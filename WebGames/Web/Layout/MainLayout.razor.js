(function () {
	const toggle = document.getElementById('menu-toggle')
	const sidebar = document.getElementById('sidebar');

	toggle.addEventListener('click', function () {
		sidebar.classList.toggle('visible');
	});
})();
