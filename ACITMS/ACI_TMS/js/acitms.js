function isValidDate(str) {
    if (!/^(0[1-9]|[1|2][0-9]|3[0|1]) (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) ([1|2][0-9][0-9][0-9])$/i.test(str)) {
        return false;
    }

    var parts = str.split(" ");
    var day = parseInt(parts[0], 10);
    var year = parseInt(parts[2], 10);

    var month;
    parts[1] = parts[1].toUpperCase();
    if (parts[1] == 'JAN') month = 1;
    else if (parts[1] == 'FEB') month = 2;
    else if (parts[1] == 'MAR') month = 3;
    else if (parts[1] == 'APR') month = 4;
    else if (parts[1] == 'MAY') month = 5;
    else if (parts[1] == 'JUN') month = 6;
    else if (parts[1] == 'JUL') month = 7;
    else if (parts[1] == 'AUG') month = 8;
    else if (parts[1] == 'SEP') month = 9;
    else if (parts[1] == 'OCT') month = 10;
    else if (parts[1] == 'NOV') month = 11;
    else if (parts[1] == 'DEC') month = 12;

    var monthLength = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    // Adjust for leap years
    if (year % 400 == 0 || (year % 100 != 0 && year % 4 == 0)) {
        monthLength[1] = 29;
    }

    // Check the range of the day
    return day > 0 && day <= monthLength[month - 1];
}

function scrollTopPage() {
    $('html,body').scrollTop(0);
}

function toFixed2dp(num) {
    return (+(Math.round(+(num + 'e' + 2)) + 'e' + -2)).toFixed(2);
}

function htmlEncode(value) {
    //create a in-memory div, set it's inner text(which jQuery automatically encodes)
    //then grab the encoded contents back out.  The div never exists on the page.
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}

