var raceToggle = 0;
var hEduToggle = 0;

function viewRegistrationForm(index) {
    if (index == 0) {
        $('#ctnSecondStep').fadeOut();
        $('#ctnThirdStep').fadeOut();
        $('#ctnFirstStep').fadeIn();
        $('#ctnForthStep').fadeOut();
        $("#ctnFifthStep").fadeOut();

        $('#btnRF1').addClass("btn-info");
        $('#btnRF2').addClass("btn-default");
        $('#btnRF3').addClass("btn-default");
        $('#btnRF4').addClass("btn-default");
        $('#btnRF5').addClass("btn-default");

        $('#btnRF1').removeClass("btn-default");
        $('#btnRF2').removeClass("btn-info");
        $('#btnRF3').removeClass("btn-info");
        $('#btnRF4').removeClass("btn-info");
        $('#btnRF5').removeClass("btn-info");

    } else if (index == 1) {
        $('#ctnFirstStep').fadeOut();
        $('#ctnThirdStep').fadeOut();
        $('#ctnSecondStep').fadeIn();
        $('#ctnForthStep').fadeOut();
        $("#ctnFifthStep").fadeOut();

        $('#btnRF1').addClass("btn-default");
        $('#btnRF2').addClass("btn-info");
        $('#btnRF3').addClass("btn-default");
        $('#btnRF4').addClass("btn-default");
        $('#btnRF5').addClass("btn-default");

        $('#btnRF1').removeClass("btn-info");
        $('#btnRF2').removeClass("btn-default");
        $('#btnRF3').removeClass("btn-info");
        $('#btnRF4').removeClass("btn-info");
        $('#btnRF5').removeClass("btn-info");

    } else if (index == 2) {
        //var phoneNo = $(".tbMobileNo").val();
        //if (phoneNo.match("/^([a-z0-9]{5,})$/g")) {
        //    alert("Match");
        //} else {
        //    alert("No");
        //}
        $('#ctnFirstStep').fadeOut();
        $('#ctnSecondStep').fadeOut();
        $('#ctnThirdStep').fadeIn();
        $('#ctnForthStep').fadeOut();
        $("#ctnFifthStep").fadeOut();

        $('#btnRF1').addClass("btn-default");
        $('#btnRF2').addClass("btn-default");
        $('#btnRF3').addClass("btn-info");
        $('#btnRF4').addClass("btn-default");
        $('#btnRF5').addClass("btn-default");

        $('#btnRF1').removeClass("btn-info");
        $('#btnRF2').removeClass("btn-info");
        $('#btnRF3').removeClass("btn-default");
        $('#btnRF4').removeClass("btn-info");
        $('#btnRF5').removeClass("btn-info");

    } else if (index == 3) {
        //var phoneNo = $(".tbMobileNo").val();
        //if (phoneNo.match("/^([a-z0-9]{5,})$/g")) {
        //    alert("Match");
        //} else {
        //    alert("No");
        //}
        $('#ctnFirstStep').fadeOut();
        $('#ctnSecondStep').fadeOut();
        $('#ctnThirdStep').fadeOut();
        $('#ctnForthStep').fadeIn();
        $("#ctnFifthStep").fadeOut();

        $('#btnRF1').addClass("btn-default");
        $('#btnRF2').addClass("btn-default");
        $('#btnRF3').addClass("btn-default");
        $('#btnRF4').addClass("btn-info");
        $('#btnRF5').addClass("btn-default");

        $('#btnRF1').removeClass("btn-info");
        $('#btnRF2').removeClass("btn-info");
        $('#btnRF3').removeClass("btn-info");
        $('#btnRF4').removeClass("btn-default");
        $('#btnRF5').removeClass("btn-info");

    } else if (index == 4) {
        $('#ctnFirstStep').fadeOut();
        $('#ctnSecondStep').fadeOut();
        $('#ctnThirdStep').fadeOut();
        $('#ctnForthStep').fadeOut();
        $("#ctnFifthStep").fadeIn();

        $('#btnRF1').addClass("btn-default");
        $('#btnRF2').addClass("btn-default");
        $('#btnRF3').addClass("btn-default");
        $('#btnRF4').addClass("btn-default");
        $('#btnRF5').addClass("btn-info");

        $('#btnRF1').removeClass("btn-info");
        $('#btnRF2').removeClass("btn-info");
        $('#btnRF3').removeClass("btn-info");
        $('#btnRF4').removeClass("btn-info");
        $('#btnRF5').removeClass("btn-default");
    }
}





//Dropdown
function raceSelected(race) {
    if (race == "Others") {
        $('#ctnOtherRace').fadeIn();
    } else {
        $('#ctnOtherRace').fadeOut();
    }
}
function educationSelected(education) {
    if (education == "Others") {
        $('#ctnOtherHEdu').fadeIn();
    } else {
        $('#ctnOtherHEdu').fadeOut();
    }
}
function otherLanguageSelected(language) {
    if (language == "Others") {
        $('#ctnOtherLanguage').fadeIn();
    } else {
        $('#ctnOtherLanguage').fadeOut();
    }
}
function otherLanguageSelected2(language) {
    if (language == "Others") {
        $('#ctnOtherLanguage2').fadeIn();
    } else {
        $('#ctnOtherLanguage2').fadeOut();
    }
}
function cPositionSelected(position) {
    if (position == "Others") {
        $('#ctnCEmploymentPositionKC').fadeOut("slow", function () {
            $('#ctnCOtherEmploymentPosition').fadeIn();
        });
    } else if (position == "Kitchen Crew") {
        $('#ctnCOtherEmploymentPosition').fadeOut("slow", function () {
            $('#ctnCEmploymentPositionKC').fadeIn();
        });
    } else {
        $('#ctnCOtherEmploymentPosition').fadeOut();
        $('#ctnCEmploymentPositionKC').fadeOut();
    }
}
function pPositionSelected(position) {
    if (position == "Others") {
        $('#ctnPEmploymentPositionKC').fadeOut("slow", function () {
            $('#ctnPOtherEmploymentPosition').fadeIn();
        });
    } else if (position == "Kitchen Crew") {
        $('#ctnPOtherEmploymentPosition').fadeOut("slow", function () {
            $('#ctnPEmploymentPositionKC').fadeIn();
        });
    } else {
        $('#ctnPOtherEmploymentPosition').fadeOut();
        $('#ctnPEmploymentPositionKC').fadeOut();
    }
}