EditorVersionSelectorBinding.prototype = new SelectorButtonBinding;
EditorVersionSelectorBinding.prototype.constructor = EditorVersionSelectorBinding;
EditorVersionSelectorBinding.superclass = SelectorButtonBinding.prototype;

EditorVersionSelectorBinding.AddNewVersionOptionId = "46e22dca-c772-44e9-b80b-34fd49fd86bf";

/**
* @class
*/
function EditorVersionSelectorBinding() {

	/**
	* @type {SystemLogger}
	*/
	this.logger = SystemLogger.getLogger("EditorVersionSelectorBinding");

	this.editorpage = null;

	/*
	* Returnable.
	*/
	return this;
}

/**
* Identifies binding.
*/
EditorVersionSelectorBinding.prototype.toString = function () {

	return "[EditorVersionSelectorBinding]";
}

/**
* @overloads {ToolBarButtonBinding#onBindingAttach}
*/
EditorVersionSelectorBinding.prototype.onBindingAttach = function () {

	EditorVersionSelectorBinding.superclass.onBindingAttach.call(this);

	this.editorpage = this.getAncestorBindingByType(EditorPageBinding);

	this.editorpage.addActionListener(EditorPageBinding.ACTION_DIRTY, this);
	this.editorpage.addActionListener(EditorPageBinding.ACTION_CLEAN, this);

	this.updateDirtyRelatedBindings();

	this._menuBodyBinding.getDescendantBindingsByType(this.MENUITEM_IMPLEMENTATION).each(function (item) {
		if (item.selectionValue === EditorVersionSelectorBinding.AddNewVersionOptionId) {
			item.setProperty("data-action","addnew");
		}
	}, this);
};

/**
 * @implements {IActionListener}
 * @overloads {Binding#handleAction}
 * @param {Action} action
 */
EditorVersionSelectorBinding.prototype.handleAction = function (action) {

	EditorVersionSelectorBinding.superclass.handleAction.call(this, action);

	switch (action.type) {

		case EditorPageBinding.ACTION_DIRTY:
		case EditorPageBinding.ACTION_CLEAN:
			this.updateDirtyRelatedBindings();
			break;
	}
}

EditorVersionSelectorBinding.prototype.getDirtyRelatedBindings = function () {

	var result = new List();

	if (!this.isSingle) {
		this._menuBodyBinding.getDescendantBindingsByType(this.MENUITEM_IMPLEMENTATION).each(function(item) {
			if (item.selectionValue === EditorVersionSelectorBinding.AddNewVersionOptionId) {
				result.add(item);
			}
		}, this);
	} else {
		if (this.singleValue === EditorVersionSelectorBinding.AddNewVersionOptionId) {
			result.add(this);
		}
	}

	return result;
}


EditorVersionSelectorBinding.prototype.updateDirtyRelatedBindings = function () {

	this.getDirtyRelatedBindings().each(function(item) {
		item.setDisabled(this.editorpage ? this.editorpage.isDirty : true);
	}, this);
}