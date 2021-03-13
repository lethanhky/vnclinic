// update page (sort, pagenumber ..)
function doCommit() {
    $('#doFilter').submit();
}

function findcolName(colName, sortType) {
    $('#sortBy').val(colName);
    $('#sortType').val(sortType);
    doCommit();
}

function updatePage(num) {
    $('#selPage').val(num);
    doCommit();
}

/** format datetime về MM/DD/YYYY */
function getDateTimeFormat(date) {
    var day = date.substring(0, 2);
    var mont = date.substring(3, 5);
    var year = date.substring(6, 10);
    var coldate = mont + "/" + day + "/" + year;
    return coldate;
}