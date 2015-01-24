setViewPort(15, 80);

var size = 200,
    color = "#000",
    colors = ["#E01B1B", "#E0841B", "#F7F307", "#07F70B"],
    dashPattern = [1];

addTetraedron(color, 3, dashPattern, size);

addPath(color, 4, dashPattern, size, [
    [0, 10, 10, 10],
    [10, 0, 10, 10],
    [10, 10, 0, 10],
    [10, 10, 10, 0]
]);
addIJK(color, 2, dashPattern, size, [30, 30, 15, 60], colors);
addVector(color, 6, dashPattern, size, [30, 30, 15, 60]);
addPoint(color, 12, "Circle", size, [30, 30, 15, 60]);