setView(0);
setTransform(0);

var size = 200,
    color = "#000",
    dashPattern = [1];

addTetraedron(color, 3, dashPattern, size);

addIJK(color, 2, dashPattern, size, [30, 30, 15, 60], [color, color, color, color]);
addPoint(color, 12, "Circle", size, [30, 30, 15, 60]);