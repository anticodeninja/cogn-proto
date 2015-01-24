setViewPort(15, -40);

var size = 100,
    length = 200,
    color = "#000",
    colors = ["#E01B1B", "#E0841B", "#F7F307", "#07F70B"],
    dashPattern = [1];

addPrism(color, 3, dashPattern, size, length);
addIJK(color, 2, dashPattern, size, length, [0.6, 30, 15, 60], colors);

addVector(color, 6, dashPattern, size, length, [0.6, 30, 15, 60]);
addPoint(color, 12, "Circle", size, length, [0.6, 30, 15, 60]);