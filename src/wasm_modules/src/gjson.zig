const std = @import("std");

const jsonMetaData = struct { len: u8 };

pub fn get(json: u8, id: u8) !void {
    std.debug.print(.{ json, id });
    // This needs to take in a string value and then outputs the value requested. It could be recognised as a struct but I think that is unneccersary and we can just do a whole bunch of recursion.
    var count = 0;
    for (id) |character| {
        count += if (character == '.') 1 else 0;
    }
    var idsplit = [count]u8;
    _ = idsplit;
}
