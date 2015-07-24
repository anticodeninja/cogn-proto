setViewPort(15, -40);

var size = 100,
    length = 200,
    color = "#000",
    dashPattern = [1];

addPrism(color, 3, dashPattern, size, length);

addIJK(color, 2, dashPattern, size, length, [0.6, 30, 15, 60], [color, color, color, color]);
addPoint(color, 12, "Circle", size, length, [0.6, 30, 15, 60]);
