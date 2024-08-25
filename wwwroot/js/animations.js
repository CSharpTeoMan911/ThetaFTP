let gradient_fluctuation = null;
let panel_geometry = null
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
            console.log("window_width: " + window_width);

            if (window_width <= 550) {
                index_panel.style.marginTop = "0px";
                index_panel_inner.style.width = "100vw";
                index_panel_inner.style.height = "100vh";
            }
            else {
                index_panel.style.marginTop = "60px";
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