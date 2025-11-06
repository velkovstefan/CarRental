$(document).ready(function () {

    $(document).on("change", ".fuelType", function () {
        let val = $(this).val();
        console.log(val);
        if (val == "Electric") {
            $(".transmission").attr("disabled", true);
            $(".transmission").val("");
        }
        else {
            $(".transmission").attr("disabled", false);
        }
    })
})