PublicationScheduleToobarButton.prototype = new SlideInButtonBinding;
PublicationScheduleToobarButton.prototype.constructor = PublicationScheduleToobarButton;
PublicationScheduleToobarButton.superclass = SlideInButtonBinding.prototype;

PublicationScheduleToobarButton.SAVEANDPUBLISH_ID = "saveandpublish";

/**
* @class
*/
function PublicationScheduleToobarButton() {

	/**
	* @type {SystemLogger}
	*/
	this.logger = SystemLogger.getLogger("PublicationScheduleToobarButton");

	/*
	* Returnable.
	*/
	return this;
}

/**
* Identifies binding.
*/
PublicationScheduleToobarButton.prototype.toString = function () {

	return "[PublicationScheduleToobarButton]";
}

/**
* @overloads {ToolBarButtonBinding#onBindingAttach}
*/
PublicationScheduleToobarButton.prototype.onBindingAttach = function () {

	PublicationScheduleToobarButton.superclass.onBindingAttach.call(this);

	var saveAndPublishLabel = this.getProperty("save-and-publsih-label");
	if (saveAndPublishLabel) {
		this.setSaveAndPublishLabel(saveAndPublishLabel);
	}
};

PublicationScheduleToobarButton.prototype.setSaveAndPublishLabel = function (title) {

	var savebutton = this.bindingWindow.bindingMap.savebutton;
	if (savebutton && savebutton instanceof ToolBarComboButtonBinding) {
		if (savebutton.getActiveMenuHandle() === PublicationScheduleToobarButton.SAVEANDPUBLISH_ID) {
			savebutton.setLabel(title);
		}

		var popupBinding = savebutton.popupBinding != null ? savebutton.popupBinding : this.getBindingForArgument(savebutton.getProperty("popup"));
		if (popupBinding) {
			var menuItemBindings = popupBinding.getDescendantBindingsByType(MenuItemBinding);
			menuItemBindings.each(
				function (menuItemBinding) {
					if (savebutton.getMenuHandle(menuItemBinding) === PublicationScheduleToobarButton.SAVEANDPUBLISH_ID) {
						menuItemBinding.setLabel(title);
					}
				}, this
			);
		}
	}
}

/**
 * Handle element update.
 * @implements {IUpdateHandler}
 * @overwrites {Binding#handleElement}
 * @param {Element} element
 * @return {boolean}
 */
PublicationScheduleToobarButton.prototype.handleElement = function (element) {

	return true;
};

/**
 * Update element.
 * @implements {IUpdateHandler}
 * @overwrites {Binding#updateElement}
 * @param {Element} element
 * @return {boolean}
 */
PublicationScheduleToobarButton.prototype.updateElement = function (element) {

	var saveAndPublishLabel = element.getAttribute("save-and-publsih-label");
	if (saveAndPublishLabel) {
		this.setProperty("save-and-publsih-label", saveAndPublishLabel);
		this.setSaveAndPublishLabel(saveAndPublishLabel);
	}
	return true;
};