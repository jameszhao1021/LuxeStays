$(document).ready(function () {
    
    loadRegisteredUserRadialChart();
})

function loadRegisteredUserRadialChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetRegisteredUserRadialChartData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            document.querySelector('#spanRegisterUserCount').innerHTML = data.totalCount;
            var sectionCurrentCount = document.createElement("span");
            if (data.hasRatioIncrease) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-up-right-circle me-1"></i> <span>' + data.countInCurrentMonth + '<span>';
            } else {
                sectionCurrentCount.className = "text-danger me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-down-right-circle me-1"></i> <span>' + data.countInCurrentMonth + '<span>';
            }
            document.querySelector('#sectionRegisteredUser').append(sectionCurrentCount);
            document.querySelector('#sectionRegisteredUser').append("since last month");

            loadRadialBarChart("registeredUsersRadialChart", data);

            $(".chart-spinner").hide();
        }
    })
}