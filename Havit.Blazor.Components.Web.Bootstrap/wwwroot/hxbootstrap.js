﻿// *** HxAutosuggest ****************************************************************

window.hxBootstrapAutosuggest_openDropdown = (selector) => {
    var el = document.querySelector(selector);
    el.setAttribute("data-bs-toggle", "dropdown");
    new bootstrap.Dropdown(el).show();
};
window.hxBootstrapAutosuggest_destroyDropdown = (selector) => {
    var el = document.querySelector(selector);
    el.removeAttribute("data-bs-toggle", "dropdown");
    var dd = bootstrap.Dropdown.getInstance(el);
    if (dd) {
        dd.hide();
        dd.dispose();
    }
};

// *** HxModal ****************************************************************

window.hxModal_show = (element, hxModalDotnetObjectReference, useStaticBackdrop, closeOnEscape) => {
    element.hxModalDotnetObjectReference = hxModalDotnetObjectReference;
    element.addEventListener('hidden.bs.modal', window.hxModal_handleModalHidden)

    var modal = new bootstrap.Modal(element, {
        backdrop: useStaticBackdrop ? "static" : false,
        keyboard: closeOnEscape
    });
    modal.show();
}

window.hxModal_hide = (element) => {
    var modal = bootstrap.Modal.getInstance(element);
    modal.hide();
}

window.hxModal_dispose = (element) => {
    var modal = bootstrap.Modal.getInstance(element);
    element.removeEventListener('hidden.bs.modal', window.hxModal_handleModalHidden);
    element.hxModalDotnetObjectReference = null;
    modal.hide();
    // modal.dispose(); modal.js:318 Uncaught TypeError: Cannot read property 'style' of null
}

window.hxModal_handleModalHidden = (event) => {
    event.target.removeEventListener('hidden.bs.modal', window.hxModal_handleModalHidden);
    event.target.hxModalDotnetObjectReference.invokeMethodAsync('HxModal_HandleModalHidden');
    event.target.hxModalDotnetObjectReference = null;

    var modal = bootstrap.Modal.getInstance(event.target);    
    modal.dispose();
};

// *** HxToast ****************************************************************

var handleToastHiddenInDotnet = (event) => {
	event.target.hxToastDotnetObjectReference.invokeMethodAsync('HxToast_HandleToastHidden');
}; 
window.hxToast_show = (element, hxToastDotnetObjectReference) => {
	element.hxToastDotnetObjectReference = hxToastDotnetObjectReference;
	element.addEventListener('hidden.bs.toast', handleToastHiddenInDotnet);
    new bootstrap.Toast(element).show();
}
window.hxToast_dispose = (element) => {
	element.removeEventListener('hidden.bs.toast', handleToastHiddenInDotnet);
}