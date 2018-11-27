<template>
    <div class="file-upload-area">
        <slot name="content"></slot>
        <label class="btn btn-default btn-file">
            <span>{{buttonText}}</span>
            <input class="input-file" multiple="" type="file">
        </label>
    </div>
</template>
<script>
export default {
    props: ["buttonText"]
};
</script>

<style lang="scss" scoped>
    .file-upload-area {
        padding: 20px;
        border: 2px dashed #a5a2a0;
        border-radius: 10px;
        text-align: center;
    }
    .input-file{
        display: none;
    }
</style>