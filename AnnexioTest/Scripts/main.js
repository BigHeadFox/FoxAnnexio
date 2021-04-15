$(document).ready(function () {
    $('#divMoreInfo').hide();
    $('#divDisable').hide();
    $('#tblCountries').DataTable();

    $('#tblCountries tbody').on('click', 'tr td', function () {        
        var th = $(this).closest('table').find('th').eq($(this).index()).text().trim();
        var td = $(this).text().trim();
        if (td.length > 0) {
            ShowData(th, td);
        }
    });

    $('body').on('click', '.borderCountry', function () {
        var cCode = $(this).text().trim();
        GetCountryNameFromCode(cCode);
    });

    $('body').on('click', '.regionCountry', function () {
        var name = $(this).text().trim();
        ShowData("name", name);
    });

    $('body').on('click', '.regionName', function () {
        var name = $(this).text().trim();
        ShowData("region", name);
    });

    $('body').on('click', '.subregionName', function () {
        var name = $(this).text().trim();
        ShowData("subregion", name);
    });

    $('body').on('click', '#divDisable', function () {
        $('#divDisable').fadeOut();
        $('#divMoreInfo').fadeOut();
    });
});

function ShowData(th, td) {
    
    var obj = new Object();
    obj.category = th;
    obj.value = td;

    $.ajax({
        url: '/Home/ShowMoreInfo',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify(obj),
        success: function (response) {
            $('#divMoreInfo').html('');

            var parsedJson = $.parseJSON(response);

            $(parsedJson).each(function (i, val) {
                $.each(val, function (k, v) {
                    $('#divMoreInfo').append('<br /><b>' + k + "</b> : " + v + '<br />');
                });
            });   

            $('#divDisable').fadeIn();
            $('#divMoreInfo').fadeIn();
        },
        error: function (xhr, status, error) {
            console.log("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText);
        }
    });
}

function GetCountryNameFromCode(cCode) {
    var obj = new Object();
    obj.cCode = cCode;
    $.ajax({
        url: '/Home/CountryCodeFromUser',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify(obj),
        success: function (response) {   
            var parsedJson = $.parseJSON(response);
            ShowData("name", parsedJson.Name);
        },
        error: function (xhr, status, error) {
            console.log("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText);
        }
    });
}