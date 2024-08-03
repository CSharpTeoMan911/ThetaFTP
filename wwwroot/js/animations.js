let gradient_fluctuation = null;
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
        gradient_fluctuation = setInterval(BackgroundGradientFluctuation, 50);
    }
}
