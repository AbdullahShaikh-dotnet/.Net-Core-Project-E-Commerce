/*! DataTables Tailwind CSS integration */

(function (factory) {
	if (typeof define === 'function' && define.amd) {
		// AMD
		define(['jquery', 'datatables.net'], function ($) {
			return factory($, window, document);
		});
	}
	else if (typeof exports === 'object') {
		// CommonJS
		var jq = require('jquery');
		var cjsRequires = function (root, $) {
			if (!$.fn.dataTable) {
				require('datatables.net')(root, $);
			}
		};

		if (typeof window === 'undefined') {
			module.exports = function (root, $) {
				if (!root) {
					// CommonJS environments without a window global must pass a
					// root. This will give an error otherwise
					root = window;
				}

				if (!$) {
					$ = jq(root);
				}

				cjsRequires(root, $);
				return factory($, root, root.document);
			};
		}
		else {
			cjsRequires(window, jq);
			module.exports = factory(jq, window, window.document);
		}
	}
	else {
		// Browser
		factory(jQuery, window, document);
	}
}(function ($, window, document) {
	'use strict';
	var DataTable = $.fn.dataTable;


	/*
	 * This is a tech preview of Tailwind CSS integration with DataTables.
	 */

	// Set the defaults for DataTables initialisation
	$.extend(true, DataTable.defaults, {
		renderer: 'tailwindcss'
	});


	// Extend DataTable classes with Tailwind CSS styles (Responsive Modern Light Theme)
	$.extend(true, DataTable.ext.classes, {
		container: "dt-container dt-tailwindcss w-full overflow-x-auto",

		search: {
			input: "border border-gray-300 rounded-md px-4 py-2 w-full max-w-xs text-gray-700 placeholder-gray-400 focus:outline-none focus:ring-1 focus:ring-gray-100 focus:border-gray-100 bg-white shadow-sm transition"
		},

		length: {
			select: "border border-gray-300 rounded-md px-4 py-2 bg-white text-gray-700 shadow-sm transition"
		},

		processing: {
			container: "dt-processing text-gray-700 font-medium py-4 text-center animate-pulse"
		},

		paging: {
			active: "font-bold bg-gray-700 text-white hover:bg-gray-700 hover:text-white transition",
			notActive: "bg-white text-gray-700",
			button: "px-4 py-2 mx-0.5 rounded-md border border-gray-300 shadow-sm hover:bg-gray-100 hover:scale-2 transition",
			first: "rounded-md",
			last: "rounded-md",
			enabled: "hover:text-gray-700 focus:ring-1 focus:ring-gray-50 transition",
			notEnabled: "text-gray-200 cursor-not-allowed hover:bg-white"
		},

		table: "w-full min-w-full border-collapse text-sm align-middle whitespace-nowrap border border-gray-200 bg-white shadow-sm rounded-lg overflow-hidden",

		thead: {
			row: "bg-gray-50 border-b border-gray-200",
			cell: "px-4 py-3 text-center text-gray-700 font-semibold uppercase tracking-wide"
		},

		tbody: {
			row: "hover:bg-gray-50 transition",
			cell: "px-4 py-3 text-gray-800 text-center"
		},

		tfoot: {
			row: "bg-gray-50 border-t border-gray-200",
			cell: "px-4 py-3 text-center text-gray-700 font-medium"
		},

		// Extra Objects Applied
		info: "text-gray-600 text-sm font-medium mt-2",

		tableWrapper: "rounded-lg shadow-md border border-gray-200 bg-white p-4 overflow-x-auto",

		rowGroup: "bg-gray-50 border-b border-gray-200",

		filter: "flex flex-col sm:flex-row space-y-2 sm:space-y-0 sm:space-x-2 bg-gray-100 p-2 rounded-lg shadow-sm",

		sortIcon: "text-gray-400 group-hover:text-gray-500 transition",

		sortActive: "bg-gray-50 text-gray-700 font-semibold",

		scrollBody: "overflow-auto max-h-96 scrollbar-thin scrollbar-thumb-gray-300 scrollbar-track-gray-100",

		responsive: {
			table: "block w-full overflow-x-auto",
			headerCell: "whitespace-nowrap px-4 py-2 font-semibold",
			bodyCell: "whitespace-nowrap px-4 py-2"
		}
	});




	DataTable.ext.renderer.pagingButton.tailwindcss = function (settings, buttonType, content, active, disabled) {
		var classes = settings.oClasses.paging;
		var btnClasses = [classes.button];

		btnClasses.push(active ? classes.active : classes.notActive);
		btnClasses.push(disabled ? classes.notEnabled : classes.enabled);

		var a = $('<a>', {
			'href': disabled ? null : '#',
			'class': btnClasses.join(' ')
		})
			.html(content);

		return {
			display: a,
			clicker: a
		};
	};

	DataTable.ext.renderer.pagingContainer.tailwindcss = function (settings, buttonEls) {
		var classes = settings.oClasses.paging;

		buttonEls[0].addClass(classes.first);
		buttonEls[buttonEls.length - 1].addClass(classes.last);

		return $('<ul/>').addClass('pagination').append(buttonEls);
	};


	DataTable.ext.renderer.layout.tailwindcss = function (settings, container, items) {
		var row = $('<div/>', {
			"class": items.full ?
				'grid grid-cols-1 gap-4 mb-4' :
				'grid grid-cols-2 gap-4 mb-4'
		})
			.appendTo(container);

		DataTable.ext.renderer.layout._forLayoutRow(items, function (key, val) {
			var klass;

			// Apply start / end (left / right when ltr) margins
			if (val.table) {
				klass = 'col-span-2';
			}
			else if (key === 'start') {
				klass = 'justify-self-start';
			}
			else if (key === 'end') {
				klass = 'col-start-2 justify-self-end';
			}
			else {
				klass = 'col-span-2 justify-self-center';
			}

			$('<div/>', {
				id: val.id || null,
				"class": klass + ' ' + (val.className || '')
			})
				.append(val.contents)
				.appendTo(row);
		});
	};


	return DataTable;
}));
