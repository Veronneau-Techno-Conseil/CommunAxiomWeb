﻿<template>
    <div class="media-field" :class="{ empty: isEmpty }">
        <div class="media-picker">
            <div class="btn-group float-right">
                <button v-on:click.prevent="select" class="btn btn-primary text-center">
                    <i class="fas fa-plus"></i>
                </button>
                <button v-on:click.prevent="remove" class="btn btn-danger text-center">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <div class="card text-left">
                <div class="card-body" v-if="isEmpty">
                    <span v-if="meta.placeholder != null" class="text-secondary">{{ meta.placeholder }}</span>
                    <span v-if="meta.placeholder == null" class="text-secondary">&nbsp;</span>
                </div>
                <div class="card-body" v-else-if="model.media.title != null">
                    <a href="#" v-on:click.prevent="piranha.preview.open(model.id)">{{ model.media.title }} ({{ model.media.filename }})</a>
                </div>
                <div class="card-body" v-else>
                    <a href="#" v-on:click.prevent="piranha.preview.open(model.id)">{{ model.media.filename }}</a>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    props: ["uid", "model", "meta"],
    methods: {
        select: function () {
            if (this.model.media != null) {
                piranha.mediapicker.open(this.update, "Image", this.model.media.folderId);
            } else {
                piranha.mediapicker.openCurrentFolder(this.update, "Image");
            }
        },
        remove: function () {
            this.model.id = null;
            this.model.media = null;
        },
        update: function (media) {
            if (media.type === "Image") {
                this.model.id = media.id;
                this.model.media = {
                    id: media.id,
                    folderId: media.folderId,
                    type: media.type,
                    filename: media.filename,
                    title: media.title,
                    contentType: media.contentType,
                    publicUrl: media.publicUrl,
                };

                // Tell parent that title has been updated
                if (this.meta.notifyChange) {
                    this.$emit('update-title', {
                        uid: this.uid,
                        title: this.model.media.title != null ? this.model.media.title + ' (' + this.model.media.filename + ')' : this.model.media.filename
                    });
                }
            } else {
                console.log("No image was selected");
            }
        }
    },
    computed: {
        isEmpty: function () {
            return this.model.media == null;
        }
    },
    mounted: function() {
        this.model.getTitle = function () {
            if (this.model.media != null) {
                return this.model.media.title != null ? this.model.media.title + ' (' + this.model.media.filename + ')' : this.model.media.filename;
            } else {
                return "No image selected";
            }
        };
    }
}
</script>