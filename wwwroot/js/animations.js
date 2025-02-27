﻿let gradient_fluctuation = null;
let panel_geometry = null;
let auth_geometry = null;
let focus_panel_geometry = null;
let gradient_value = 90;
let substract = true;

function BackgroundGradientFluctuation() {
    let body = document.getElementById("body");

    if (body != null) {
        body.style.cssText = "; background: linear-gradient(to right, #626161 " + gradient_value + "%, #dbd7d7); -webkit-background: linear-gradient(to right, #626161 " + gradient_value + "%, #dbd7d7); background: --moz-linear-gradient(to right, #626161 " + gradient_value + "%, #dbd7d7);";

        if (gradient_value == 0) {
            substract = false;
        }
        else if (gradient_value == 90) {
            substract = true;
        }

        switch (substract) {
            case true:
                gradient_value--;
                break;
            case false:
                gradient_value++;
                break;
        }
    }
}

export function InitBackgroundGradientFluctuation() {
    if (gradient_fluctuation === null) {
        gradient_fluctuation = setInterval(() => { BackgroundGradientFluctuation() }, 50);
    }
}


function MainPanelGeometry() {
    let index_panel = document.getElementById("index_panel");
    let index_panel_inner = document.getElementById("index_panel_inner");
    if (index_panel !== null) {
        if (index_panel_inner !== null) {
            let window_width = window.innerWidth;
            if (window_width <= 1000 || navigator.maxTouchPoints > 0) {
                index_panel.style.marginTop = "0px";
                index_panel.style.marginBottom = "0px";
                index_panel_inner.style.width = "100vw";
                index_panel_inner.style.height = "100vh";
            }
            else {
                index_panel.style.marginTop = "60px";
                index_panel.style.marginBottom = "30px";
                index_panel_inner.style.width = "84vw";
                index_panel_inner.style.height = "78vh";
            }
        }
    }
}

export function InitMainPanelGeometry() {
    if (panel_geometry === null) {
        panel_geometry = setInterval(() => { MainPanelGeometry() }, 50);
    }
}


function FocusPanelGeometry() {
    let focus_panel_collection = document.getElementsByClassName("focus_panel");
    let page_collection = document.getElementsByClassName("body");

    if (focus_panel_collection !== null && focus_panel_collection.length === 1) {
        if (page_collection !== null) {
            let focus_panel = focus_panel_collection.item(0);
            let page = page_collection.item(0);
            if (focus_panel !== null) {
                if (page !== null) {
                    focus_panel.style.height = page.offsetHeight + "px";
                    focus_panel.style.width = page.offsetWidth + "px";
                }
            }
        }
    }
}

export function InitFocusPanelGeometry() {
    if (focus_panel_geometry == null) {
        focus_panel_geometry = setInterval(() => { FocusPanelGeometry() }, 50);
    }
}
